using ClubManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Linq;

namespace ClubManagementSystem.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IConfiguration _configuration;
        
        public AuthenticationController(IAuthenticationService authenticationService, IConfiguration configuration)
        {
            _authenticationService = authenticationService;
            _configuration = configuration;
        }
        [HttpPost("login")]
        public async Task<ActionResult> Login([FromBody] Service.Models.Request.LoginRequest loginRequest)
        {
            try
            {
                if(!ModelState.IsValid)
                {
                    return Unauthorized(ModelState);
                }
                var result = await _authenticationService.Login(loginRequest);
                if (result.Success)
                {
                    return Ok(result.Data);
                }
                else
                {
                    return Unauthorized(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("google-login")]
        public IActionResult GoogleLogin()
        {
            try
            {
                var googleConfig = _configuration.GetSection("Google");
                var clientId = googleConfig["ClientId"];
                
                if (string.IsNullOrEmpty(clientId))
                {
                    return BadRequest("Google OAuth ClientId is not configured");
                }

                // Tạo redirect URI (backend callback URL)
                var redirectUri = $"{Request.Scheme}://{Request.Host}/api/authentication/google-callback";
                
                // Google OAuth URL với các scopes cần thiết
                var googleAuthUrl = $"https://accounts.google.com/o/oauth2/v2/auth?" +
                    $"client_id={Uri.EscapeDataString(clientId)}&" +
                    $"redirect_uri={Uri.EscapeDataString(redirectUri)}&" +
                    $"response_type=code&" +
                    $"scope={Uri.EscapeDataString("openid email profile")}&" +
                    $"access_type=offline&" +
                    $"prompt=consent";

                return Redirect(googleAuthUrl);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback([FromQuery] string? code, [FromQuery] string? error)
        {
            try
            {
                // Lấy frontend URL từ config
                var corsOrigins = _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };
                var frontendUrl = corsOrigins.FirstOrDefault() ?? "http://localhost:3000";

                // Kiểm tra nếu có lỗi từ Google
                if (!string.IsNullOrEmpty(error))
                {
                    return Redirect($"{frontendUrl}/login?error=access_denied");
                }

                if (string.IsNullOrEmpty(code))
                {
                    return Redirect($"{frontendUrl}/login?error=no_code");
                }

                // Tạo redirect URI (phải khớp với redirect URI đã config trong Google Console)
                var redirectUri = $"{Request.Scheme}://{Request.Host}/api/authentication/google-callback";

                // Gọi service để xử lý login
                var result = await _authenticationService.LoginGoogle(code, redirectUri);

                if (result.Success && result.Data != null)
                {
                    // Redirect về frontend với token trong query string
                    return Redirect($"{frontendUrl}/auth/callback?token={Uri.EscapeDataString(result.Data.AccessToken)}&expiresAt={Uri.EscapeDataString(result.Data.ExpiresAt.ToString("o"))}");
                }
                else
                {
                    return Redirect($"{frontendUrl}/login?error=login_failed&message={Uri.EscapeDataString(result.Message ?? "Login failed")}");
                }
            }
            catch (Exception ex)
            {
                var corsOrigins = _configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? new[] { "http://localhost:3000" };
                var frontendUrl = corsOrigins.FirstOrDefault() ?? "http://localhost:3000";
                return Redirect($"{frontendUrl}/login?error=server_error&message={Uri.EscapeDataString(ex.Message)}");
            }
        }

        // Endpoint test - chỉ dùng cho development, trả về JSON thay vì redirect
        [HttpGet("google-callback-test")]
        public async Task<ActionResult> GoogleCallbackTest([FromQuery] string? code, [FromQuery] string? error)
        {
            try
            {
                // Kiểm tra nếu có lỗi từ Google
                if (!string.IsNullOrEmpty(error))
                {
                    return BadRequest(new { Success = false, Message = "Access denied", Error = error });
                }

                if (string.IsNullOrEmpty(code))
                {
                    return BadRequest(new { Success = false, Message = "Authorization code is required" });
                }

                // Tạo redirect URI (phải khớp với redirect URI đã config trong Google Console)
                var redirectUri = $"{Request.Scheme}://{Request.Host}/api/authentication/google-callback";

                // Gọi service để xử lý login
                var result = await _authenticationService.LoginGoogle(code, redirectUri);

                if (result.Success && result.Data != null)
                {
                    return Ok(result);
                }
                else
                {
                    return Unauthorized(result);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, Message = $"Internal server error: {ex.Message}" });
            }
        }
    }
}
