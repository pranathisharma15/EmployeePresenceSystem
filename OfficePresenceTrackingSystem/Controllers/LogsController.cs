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
        private readonly IPresenceService _presenceService; // Service for fetching presence logs
        private readonly ILogger<LogsController> _logger;   // Logger for tracking actions and errors in this controller

        public LogsController(  // Constructor with dependency injection for presence service and logger
            IPresenceService presenceService,
            ILogger<LogsController> logger)
        {
            _presenceService = presenceService;
            _logger = logger;
        }

        // ADMIN - see all processed logs
        [Authorize(Roles = "Admin")]
        [HttpGet("all")]
        public async Task<IActionResult> GetAllLogs()   // Action to get all presence logs, accessible only by Admin role
        {
            try
            {
                var logs = await _presenceService.GetAllPresenceAsync();

                if (logs == null || !logs.Any())
                {
                    _logger.LogInformation("No presence logs found");
                    return NotFound(new
                    {
                        message = "No presence logs available"
                    });
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all logs");

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while fetching logs"
                });
            }
        }

        // EMPLOYEE - get own logs
        [Authorize(Roles = "Employee,Admin")]
        [HttpGet("my")]
        public async Task<IActionResult> GetMyLogs([FromQuery] string name) // Action to get logs for a specific employee, accessible by both Employee and Admin roles. The employee name is passed as a query parameter.
        {
            try
            {
                if (string.IsNullOrWhiteSpace(name))
                {
                    _logger.LogWarning("Employee name query parameter missing");
                    return BadRequest(new
                    {
                        message = "Employee name is required"
                    });
                }

                var logs = await _presenceService
                    .GetPresenceByEmployeeNameAsync(name.Trim());

                if (logs == null || !logs.Any())
                {
                    _logger.LogInformation("No logs found for employee {Name}", name);
                    return NotFound(new
                    {
                        message = $"No logs found for employee '{name}'"
                    });
                }

                return Ok(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching logs for employee {Name}", name);

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while fetching employee logs"
                });
            }
        }
    }
}