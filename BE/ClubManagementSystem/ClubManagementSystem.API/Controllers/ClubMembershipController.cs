using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementSystem.API.Controllers
{
    [Route("api/club-memberships")]
    [ApiController]
    public class ClubMembershipController : ControllerBase
    {
        private readonly IClubMembershipService _clubMembershipService;

        public ClubMembershipController(IClubMembershipService clubMembershipService)
        {
            _clubMembershipService = clubMembershipService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllClubMemberships([FromQuery] ClubMembershipFilterRequest filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var memberships = await _clubMembershipService.GetAllClubMemberships(filter, page, pageSize);
                return Ok(memberships);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ClubMembershipResponse>> GetClubMembershipById(int id)
        {
            try
            {
                var result = await _clubMembershipService.GetClubMembershipByIdAsync(id);
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
        public async Task<ActionResult<ClubMembershipResponse>> CreateClubMembership([FromBody] ClubMembershipRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _clubMembershipService.CreateAsync(request);
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetAllClubMemberships), new { id = result.Data.Id }, result.Data);
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
        public async Task<ActionResult<ClubMembershipResponse>> UpdateClubMembership(int id, [FromBody] ClubMembershipRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _clubMembershipService.UpdateAsync(id, request);
                if (result.Success)
                {
                    return Ok(result.Data);
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

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteClubMembership(int id)
        {
            try
            {
                var result = await _clubMembershipService.DeleteAsync(id);
                if (result.Success)
                {
                    return NoContent();
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
    }
}


