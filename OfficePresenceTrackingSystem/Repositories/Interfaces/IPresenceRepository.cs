using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Repositories.Interfaces
{
    public interface IPresenceRepository
    {
        Task SaveWifiLogsAsync(List<WifiLog> logs);

        Task SaveEmployeesAsync(List<Employee> employees);

        Task<List<WifiLog>> GetWifiLogsAsync();

        Task<List<Employee>> GetEmployeesAsync();

        Task SavePresenceRecordsAsync(List<PresenceRecord> records);

        Task<List<PresenceRecord>> GetPresenceRecordsAsync();
    }
}