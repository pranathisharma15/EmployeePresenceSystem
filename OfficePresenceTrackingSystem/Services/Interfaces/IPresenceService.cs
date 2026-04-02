using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Services.Interfaces
{
    public interface IPresenceService   // This interface defines the contract for a presence service, which is responsible for handling the business logic related to employee presence tracking in the application. It includes methods for uploading WiFi logs and employee mappings, as well as fetching presence records based on various criteria. Implementing this interface allows for a consistent way to manage presence-related operations across the application, ensuring that the service can be easily used by controllers and other components that require access to presence data.
    {
        Task UploadWifiLogsAsync(IFormFile file);

        Task UploadMappingsAsync(IFormFile file);

        Task<List<PresenceRecord>> GetPresenceAsync(DateTime? date = null);

        Task<List<PresenceRecord>> GetAllPresenceAsync();

        Task<List<PresenceRecord>> GetPresenceByEmployeeNameAsync(string employeeName);
    }
}