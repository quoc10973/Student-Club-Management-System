using System;
using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementSystem.API.Controllers
{
    [Route("api/apply-registrations")]
    [ApiController]
    public class ApplyRegistrationController : ControllerBase
    {
        private readonly IApplyRegistrationService _applyRegistrationService;

        public ApplyRegistrationController(IApplyRegistrationService applyRegistrationService)
        {
            _applyRegistrationService = applyRegistrationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllApplyRegistrations([FromQuery] ApplyRegistrationFilterRequest filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var applies = await _applyRegistrationService.GetAllApplyRegistrations(filter, page, pageSize);
                return Ok(applies);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApplyRegistrationResponse>> GetApplyRegistrationById(int id)
        {
            try
            {
                var result = await _applyRegistrationService.GetApplyRegistrationByIdAsync(id);
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
        public async Task<ActionResult<ApplyRegistrationResponse>> CreateApplyRegistration([FromBody] ApplyRegistrationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _applyRegistrationService.CreateAsync(request);
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetAllApplyRegistrations), new { id = result.Data.Id }, result.Data);
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
        public async Task<ActionResult<ApplyRegistrationResponse>> UpdateApplyRegistration(int id, [FromBody] ApplyRegistrationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _applyRegistrationService.UpdateAsync(id, request);
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
        public async Task<ActionResult> DeleteApplyRegistration(int id)
        {
            try
            {
                var result = await _applyRegistrationService.DeleteAsync(id);
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




