using Microsoft.AspNetCore.Mvc;
using OfficePresenceTrackingSystem.Services.Interfaces;

namespace OfficePresenceTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PresenceController : ControllerBase
    {
        private readonly IPresenceService _presenceService;
        private readonly ILogger<PresenceController> _logger;

        public PresenceController(
            IPresenceService presenceService,
            ILogger<PresenceController> logger)
        {
            _presenceService = presenceService;
            _logger = logger;
        }

        [HttpPost("upload-wifi")]
        public async Task<IActionResult> UploadWifi(IFormFile file)
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

                await _presenceService.UploadWifiLogsAsync(file);

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

        [HttpPost("upload-mapping")]
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
        public async Task<IActionResult> GetPresence()
        {
            try
            {
                var result = await _presenceService.GetPresenceAsync();

                if (result == null || !result.Any())
                {
                    _logger.LogInformation("No presence data found");
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