using Microsoft.AspNetCore.Mvc;
using OfficePresenceTrackingSystem.Services.Interfaces;

namespace OfficePresenceTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PresenceController : ControllerBase
    {
        private readonly IPresenceService _presenceService;

        public PresenceController(IPresenceService presenceService)
        {
            _presenceService = presenceService;
        }

        [HttpPost("upload-wifi")]
        public async Task<IActionResult> UploadWifi(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            await _presenceService.UploadWifiLogsAsync(file);
            return Ok(new { message = "WiFi logs uploaded successfully" });
        }

        [HttpPost("upload-mapping")]
        public async Task<IActionResult> UploadMapping(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Invalid file");

            await _presenceService.UploadMappingsAsync(file);
            return Ok(new { message = "Mappings uploaded successfully" });
        }

        [HttpGet]
        public async Task<IActionResult> GetPresence()
        {
            var result = await _presenceService.GetPresenceAsync();
            return Ok(result);
        }
    }
}