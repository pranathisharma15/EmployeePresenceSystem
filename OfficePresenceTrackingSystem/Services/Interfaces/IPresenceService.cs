using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Services.Interfaces
{
    public interface IPresenceService
    {
        Task UploadWifiLogsAsync(IFormFile file);

        Task UploadMappingsAsync(IFormFile file);

        Task<List<PresenceRecord>> GetPresenceAsync(DateTime? date = null);

        Task<List<PresenceRecord>> GetAllPresenceAsync();

        Task<List<PresenceRecord>> GetPresenceByEmployeeNameAsync(string employeeName);
    }
}