using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Repository.Repositories;
using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Implement
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAccountService _accountService;
        private readonly IStudentService _studentService;
        private readonly DepartmentRepository _departmentRepository;
        private readonly IConfiguration _configuration;
        public AuthenticationService(IAccountService accountService, IStudentService studentService, DepartmentRepository departmentRepository, IConfiguration configuration)
        {
            _accountService = accountService;
            _studentService = studentService;
            _departmentRepository = departmentRepository;
            _configuration = configuration;
        }
        public Task<JwtTokenResult> GenerateToken(Account account)
        {
            var jwtConfig = _configuration.GetSection("JwtConfig");

            var issuer = jwtConfig["Issuer"];
            var audience = jwtConfig["Audience"];
            var key = jwtConfig["Key"];
            var expiryIn = DateTime.Now.AddMinutes(Double.Parse(jwtConfig["ExpireMinutes"]));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim("accountId", account.Id.ToString()),
                    new Claim(ClaimTypes.Name, account.Username),
                    new Claim(ClaimTypes.Role, account.Role.ToString())
                }),
                Expires = expiryIn,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            var accessToken = tokenHandler.WriteToken(token);
            var accessTokenResult = new JwtTokenResult
            {
                Token = accessToken,
                ExpiresAt = expiryIn
            };
            return Task.FromResult(accessTokenResult);
        }

        public async Task<ApiResponse<LoginResponse>> Login(LoginRequest loginRequest)
        {
            try
            {
                if (loginRequest == null || string.IsNullOrEmpty(loginRequest.Username) || string.IsNullOrEmpty(loginRequest.Password))
                {
                    return new ApiResponse<LoginResponse>()
                    {
                        Success = false,
                        Message = "Invalid login request",
                        Data = null
                    };
                }
                var user = await _accountService.GetByUsernameAsync(loginRequest.Username);
                if (user != null)
                {
                    var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginRequest.Password, user.PasswordHash);
                    if (!isPasswordValid)
                    {
                        return new ApiResponse<LoginResponse>
                        {
                            Success = false,
                            Message = "Invalid Username or Password",
                            Data = null
                        };
                    }
                    // acess token
                    var jwtTokenResult = await GenerateToken(user);

                    var response = user.Adapt<LoginResponse>();
                    response.AccessToken = jwtTokenResult.Token;
                    response.ExpiresAt = jwtTokenResult.ExpiresAt;

                    return new ApiResponse<LoginResponse>
                    {
                        Success = true,
                        Message = "Login successfully",
                        Data = response
                    };
                }
                return new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "Invalid Username or Password",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponse>()
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<ApiResponse<LoginResponse>> LoginGoogle(string code, string redirectUri)
        {
            try
            {
                if (string.IsNullOrEmpty(code))
                {
                    return new ApiResponse<LoginResponse>()
                    {
                        Success = false,
                        Message = "Authorization code is required",
                        Data = null
                    };
                }

                var googleConfig = _configuration.GetSection("Google");
                var clientId = googleConfig["ClientId"];
                var clientSecret = googleConfig["ClientSecret"];

                if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
                {
                    return new ApiResponse<LoginResponse>()
                    {
                        Success = false,
                        Message = "Google OAuth configuration is missing",
                        Data = null
                    };
                }

                // 1. Đổi authorization code lấy access token
                using var httpClient = new HttpClient();
                var tokenRequest = new
                {
                    code = code,
                    client_id = clientId,
                    client_secret = clientSecret,
                    redirect_uri = redirectUri,
                    grant_type = "authorization_code"
                };

                var tokenResponse = await httpClient.PostAsJsonAsync("https://oauth2.googleapis.com/token", tokenRequest);
                if (!tokenResponse.IsSuccessStatusCode)
                {
                    return new ApiResponse<LoginResponse>()
                    {
                        Success = false,
                        Message = "Failed to exchange authorization code for token",
                        Data = null
                    };
                }

                var tokenContent = await tokenResponse.Content.ReadAsStringAsync();
                var tokenData = JsonSerializer.Deserialize<JsonElement>(tokenContent);
                var accessToken = tokenData.GetProperty("access_token").GetString();

                if (string.IsNullOrEmpty(accessToken))
                {
                    return new ApiResponse<LoginResponse>()
                    {
                        Success = false,
                        Message = "Failed to get access token from Google",
                        Data = null
                    };
                }

                // 2. Lấy thông tin user từ Google
                httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
                var userInfoResponse = await httpClient.GetAsync("https://www.googleapis.com/oauth2/v2/userinfo");
                
                if (!userInfoResponse.IsSuccessStatusCode)
                {
                    return new ApiResponse<LoginResponse>()
                    {
                        Success = false,
                        Message = "Failed to get user information from Google",
                        Data = null
                    };
                }

                var userInfoContent = await userInfoResponse.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<JsonElement>(userInfoContent);
                
                var email = userInfo.GetProperty("email").GetString();
                var fullName = userInfo.GetProperty("name").GetString();
                var picture = userInfo.TryGetProperty("picture", out var picElement) ? picElement.GetString() : null;

                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(fullName))
                {
                    return new ApiResponse<LoginResponse>()
                    {
                        Success = false,
                        Message = "Failed to get email or name from Google",
                        Data = null
                    };
                }

                // 3. Tìm hoặc tạo account
                var account = await _accountService.GetByEmailAsync(email);

                if (account == null)
                {
                    // Tạo username từ email
                    var username = email.Split('@')[0];
                    var existingAccount = await _accountService.GetByUsernameAsync(username);
                    if (existingAccount != null)
                    {
                        username = $"{username}_{new Random().Next(1000, 9999)}";
                    }

                    // Tạo account mới
                    var newAccountRequest = new AccountRequest
                    {
                        Username = username,
                        Email = email,
                        FullName = fullName,
                        Password = Guid.NewGuid().ToString(), // Random password
                        Role = "User",
                        Status = "Active",
                        Phone = "0000000000"
                    };

                    var createResult = await _accountService.CreateAsync(newAccountRequest);
                    if (!createResult.Success)
                    {
                        return new ApiResponse<LoginResponse>
                        {
                            Success = false,
                            Message = "Failed to create account from Google login",
                            Data = null
                        };
                    }

                    account = await _accountService.GetByEmailAsync(email);
                    if (account == null)
                    {
                        return new ApiResponse<LoginResponse>
                        {
                            Success = false,
                            Message = "Account created but could not be retrieved",
                            Data = null
                        };
                    }

                    // Tạo Student tương ứng cho account mới
                    // Logic: Lấy 8 ký tự cuối từ username (hoặc tất cả nếu < 8 ký tự), viết hoa 2 chữ cái đầu để tạo code
                    // Tìm department theo CodeName trùng với 2 chữ cái đầu đã viết hoa
                    Deparment department = null;
                    string studentCode = null;

                    // Lấy 8 ký tự cuối cùng từ username (hoặc tất cả nếu username < 8 ký tự)
                    var last8Chars = username.Length >= 8 
                        ? username.Substring(username.Length - 8)
                        : username;
                    
                    // Viết hoa 2 chữ cái đầu của chuỗi đó (nếu có ít nhất 2 ký tự và là chữ cái)
                    if (last8Chars.Length >= 2 && char.IsLetter(last8Chars[0]) && char.IsLetter(last8Chars[1]))
                    {
                        var firstTwoUpper = last8Chars.Substring(0, 2).ToUpper();
                        var remainingChars = last8Chars.Substring(2);
                        studentCode = firstTwoUpper + remainingChars;

                        // Tìm department theo CodeName trùng với 2 chữ cái đầu đã viết hoa
                        department = await _departmentRepository.GetByCodeNameAsync(firstTwoUpper);
                    }

                    // Nếu không tìm thấy department hoặc không thể tạo code từ username
                    // Fallback về logic cũ: set default giá trị
                    if (department == null || string.IsNullOrEmpty(studentCode))
                    {
                        department = await _departmentRepository.GetFirstActiveDepartmentAsync();
                        if (department == null)
                        {
                            department = await _departmentRepository.GetFirstDepartmentAsync();
                        }
                        
                        // Nếu vẫn không có department, dùng code mặc định
                        if (department != null)
                        {
                            var departmentCode = !string.IsNullOrEmpty(department.CodeName) 
                                ? department.CodeName 
                                : "SE";
                            studentCode = $"{departmentCode}0000";
                        }
                        else
                        {
                            studentCode = "SE0000";
                        }
                    }

                    if (department != null && !string.IsNullOrEmpty(studentCode))
                    {
                        var studentRequest = new StudentRequest
                        {
                            AccountId = account.Id,
                            DeparmentId = department.Id,
                            Status = "Active",
                            Code = studentCode
                        };

                        var createStudentResult = await _studentService.CreateAsync(studentRequest);
                        if (!createStudentResult.Success)
                        {
                            // Log warning nhưng không fail login vì account đã được tạo
                            // Student có thể được tạo thủ công sau hoặc update department thông qua API
                        }
                    }
                }

                // 4. Kiểm tra account status
                if (account.Status != "Active")
                {
                    return new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "Account is not active",
                        Data = null
                    };
                }

                // 5. Generate JWT token
                var jwtTokenResult = await GenerateToken(account);

                var response = account.Adapt<LoginResponse>();
                response.AccessToken = jwtTokenResult.Token;
                response.ExpiresAt = jwtTokenResult.ExpiresAt;

                return new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Message = "Google login successfully",
                    Data = response
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<LoginResponse>()
                {
                    Success = false,
                    Message = $"Error: {ex.Message}",
                    Data = null
                };
            }
        }
    }
}
