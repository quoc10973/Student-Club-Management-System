using System;
using ClubManagementSystem.Service.Interface;
using ClubManagementSystem.Service.Models.Request;
using ClubManagementSystem.Service.Models.Response;
using Microsoft.AspNetCore.Mvc;

namespace ClubManagementSystem.API.Controllers
{
    [Route("api/registrations")]
    [ApiController]
    public class RegistrationController : ControllerBase
    {
        private readonly IRegistrationService _registrationService;

        public RegistrationController(IRegistrationService registrationService)
        {
            _registrationService = registrationService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetAllRegistrations([FromQuery] RegistrationFilterRequest filter, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var registrations = await _registrationService.GetAllRegistrations(filter, page, pageSize);
                return Ok(registrations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RegistrationResponse>> GetRegistrationById(int id)
        {
            try
            {
                var result = await _registrationService.GetRegistrationByIdAsync(id);
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
        public async Task<ActionResult<RegistrationResponse>> CreateRegistration([FromBody] RegistrationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _registrationService.CreateAsync(request);
                if (result.Success)
                {
                    return CreatedAtAction(nameof(GetAllRegistrations), new { id = result.Data.Id }, result.Data);
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
        public async Task<ActionResult<RegistrationResponse>> UpdateRegistration(int id, [FromBody] RegistrationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var result = await _registrationService.UpdateAsync(id, request);
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
        public async Task<ActionResult> DeleteRegistration(int id)
        {
            try
            {
                var result = await _registrationService.DeleteAsync(id);
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




