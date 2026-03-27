using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OfficePresenceTrackingSystem.Services.Interfaces;

namespace OfficePresenceTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/logs")]
    [Authorize]
    public class LogsController : ControllerBase
    {
        private readonly IPresenceService _presenceService;

        public LogsController(IPresenceService presenceService)
        {
            _presenceService = presenceService;
        }

        // ✅ ADMIN → See all logs
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLogs()
        {
            var logs = await _presenceService.GetAllPresenceAsync();
            return Ok(logs);
        }

        // ✅ EMPLOYEE → Enter name and get logs
        [Authorize(Roles = "Employee,Admin")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyLogs([FromQuery] string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return BadRequest("Name is required");

            var logs = await _presenceService.GetPresenceByEmployeeNameAsync(name.Trim());

            return Ok(logs);
        }
    }
}