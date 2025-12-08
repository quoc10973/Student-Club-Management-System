using ClubManagementSystem.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementSystem.API.Controllers
{
    [Route("api/authentication")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
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
    }
}
