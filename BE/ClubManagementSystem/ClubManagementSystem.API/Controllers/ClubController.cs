using System;
using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementSystem.API.Controllers
{
    [Route("api/clubs")]
    [ApiController]
    public class ClubController : ControllerBase
    {
        private readonly IClubService _clubService;

        public ClubController(IClubService clubService)
        {
            _clubService = clubService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllClubs([FromQuery] ClubFilterRequest filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var clubs = await _clubService.GetAllClubs(filter, page, pageSize);
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
    }
}
