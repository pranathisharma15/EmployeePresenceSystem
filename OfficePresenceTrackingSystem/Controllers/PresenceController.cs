using Microsoft.AspNetCore.Mvc;
using OfficePresenceTrackingSystem.Models;
using OfficePresenceTrackingSystem.Services.Interfaces;

namespace OfficePresenceTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PresenceController : ControllerBase    //  This controller handles presence-related operations, such as uploading WiFi logs and mappings, and fetching presence data.
    {
        private readonly IPresenceService _presenceService; //  Service for handling presence-related business logic
        private readonly ILogger<PresenceController> _logger;

        public PresenceController(  // Constructor with dependency injection for presence service and logger
            IPresenceService presenceService,
            ILogger<PresenceController> logger)
        {
            _presenceService = presenceService;
            _logger = logger;
        }

        [HttpPost("upload-wifi")]   // This action handles the uploading of WiFi log files. It expects a file to be sent in the request and processes it using the presence service.
        public async Task<IActionResult> UploadWifi(IFormFile file) // Action to upload WiFi logs, accessible via POST request to "api/presence/upload-wifi". The file is expected to be sent as form data.
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("WiFi upload failed: file is null or empty");
                    return BadRequest(new
                    {
                        message = "Invalid WiFi log file"
                    });
                }

                await _presenceService.UploadWifiLogsAsync(file);   // Calls the service to process the uploaded WiFi log file

                return Ok(new
                {
                    message = "WiFi logs uploaded successfully"
                });
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid WiFi CSV format");
                return BadRequest(new
                {
                    message = "Invalid WiFi CSV format"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during WiFi upload");

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while uploading WiFi logs"
                });
            }
        }

        [HttpPost("upload-mapping")]    // This action handles the uploading of employee mapping files. It expects a file to be sent in the request and processes it using the presence service.
        public async Task<IActionResult> UploadMapping(IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                {
                    _logger.LogWarning("Mapping upload failed: file is null or empty");
                    return BadRequest(new
                    {
                        message = "Invalid mapping file"
                    });
                }

                await _presenceService.UploadMappingsAsync(file);

                return Ok(new
                {
                    message = "Mappings uploaded successfully"
                });
            }
            catch (FormatException ex)
            {
                _logger.LogError(ex, "Invalid mapping CSV format");
                return BadRequest(new
                {
                    message = "Invalid mapping CSV format"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during mapping upload");

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while uploading mappings"
                });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetPresence([FromQuery] DateTime? date = null)
        {
            try
            {
                var result = await _presenceService.GetPresenceAsync(date);

                if (result == null || !result.Any())
                {
                    _logger.LogInformation("No presence data found for the selected date");
                    return NotFound(new
                    {
                        message = "No presence data available"
                    });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching presence data");

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while fetching presence data"
                });
            }
        }
    }
}