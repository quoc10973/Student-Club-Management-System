using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace ClubManagementSystem.API.Controllers
{
    [Route("api/clubs")]
    [Authorize]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;

        public ClubController(IClubService clubService)
        {
            _clubService = clubService;
        }
        public class SetLeaderRequest
        {
            public int StudentId { get; set; }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllClubs([FromQuery] ClubFilterRequest filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                // Lấy accountId và role từ JWT token (nếu có)
                int? accountId = null;
                string? role = null;
                
                if (User.Identity?.IsAuthenticated == true)
                {
                    var accountIdClaim = User.FindFirst("accountId") ?? User.FindFirst("Id");
                    if (accountIdClaim != null && int.TryParse(accountIdClaim.Value, out var parsedAccountId))
                    {
                        accountId = parsedAccountId;
                    }
                    
                    var roleClaim = User.FindFirst(System.Security.Claims.ClaimTypes.Role) ?? User.FindFirst("role");
                    if (roleClaim != null)
                    {
                        role = roleClaim.Value;
                    }
                }

                var clubs = await _clubService.GetAllClubs(filter, page, pageSize, accountId, role);
                return Ok(clubs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClubResponse>> GetClubById(int id)
        {
            try
            {
                var result = await _clubService.GetClubByIdAsync(id);
                if (result.Success)
                {
                    return Ok(result.Data);
                }
                else
                {
                    return NotFound(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<ActionResult<ClubResponse>> CreateClub([FromBody] ClubRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _clubService.CreateAsync(request);
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetAllClubs), new { id = result.Data.Id }, result.Data);
                }
                else
                {
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ClubResponse>> UpdateClub(int id, [FromBody] ClubRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var result = await _clubService.UpdateAsync(id, request);
                if (result.Success)
                {
                    return Ok(result.Data);
                }
                else
                {
                    if (result.Message != null && result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    {
                        return NotFound(result.Message);
                    }
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClub(int id)
        {
            try
            {
                var result = await _clubService.DeleteAsync(id);
                if (result.Success)
                {
                    return NoContent();
                }
                else
                {
                    if (result.Message != null && result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase))
                    {
                        return NotFound(result.Message);
                    }
                    return BadRequest(result.Message);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("{clubId}/set-leader")]
        public async Task<ActionResult> SetLeader(int clubId, [FromBody] SetLeaderRequest request)
        {
            try
            {
                var accountIdClaim = User.FindFirst("accountId") ?? User.FindFirst("Id");
                if (accountIdClaim == null)
                {
                    return Unauthorized("accountId claim not found in token.");
                }

                if (!int.TryParse(accountIdClaim.Value, out var adminAccountId))
                {
                    return BadRequest("Invalid accountId in token.");
                }

                var result = await _clubService.SetLeaderAsync(clubId, request.StudentId, adminAccountId);
                if (result.Success)
                {
                    return Ok(result.Message);
                }

                if (result.Message != null &&
                    (result.Message.Contains("not found", StringComparison.OrdinalIgnoreCase) ||
                     result.Message.Contains("does not belong", StringComparison.OrdinalIgnoreCase)))
                {
                    return NotFound(result.Message);
                }

                return BadRequest(result.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
