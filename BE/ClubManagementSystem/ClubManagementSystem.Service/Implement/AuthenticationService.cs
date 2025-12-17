using ClubManagementSystem.Repository.Entities;
using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Common;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Mapster;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ClubManagementSystem.Service.Implement
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IAccountService _accountService;
        private readonly IConfiguration _configuration;
        public AuthenticationService(IAccountService accountService, IConfiguration configuration)
        {
            _accountService = accountService;
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
    }
}
