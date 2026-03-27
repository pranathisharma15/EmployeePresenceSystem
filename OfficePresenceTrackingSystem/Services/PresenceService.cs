using OfficePresenceTrackingSystem.Constants;
using OfficePresenceTrackingSystem.Helpers;
using OfficePresenceTrackingSystem.Models;
using OfficePresenceTrackingSystem.Repositories.Interfaces;
using OfficePresenceTrackingSystem.Services.Interfaces;

namespace OfficePresenceTrackingSystem.Services
{
    public class PresenceService : IPresenceService
    {
        private readonly IPresenceRepository _repository;

        public PresenceService(IPresenceRepository repository)
        {
            _repository = repository;
        }

        public async Task UploadWifiLogsAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var logs = CsvParser.ParseWifiLogs(stream);

            await _repository.SaveWifiLogsAsync(logs);
        }

        public async Task UploadMappingsAsync(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            var employees = CsvParser.ParseMappings(stream);

            await _repository.SaveEmployeesAsync(employees);
        }

        public async Task<List<PresenceRecord>> GetPresenceAsync()
        {
            var logs = await _repository.GetWifiLogsAsync();
            var employees = await _repository.GetEmployeesAsync();

            var presenceList = new List<PresenceRecord>();

            foreach (var emp in employees)
            {
                // ✅ Direct match (already normalized in CsvParser)
                var empLogs = logs
                    .Where(l => l.HostName == emp.SerialNumber)
                    .OrderBy(l => l.StartTime)
                    .ToList();

                if (!empLogs.Any())
                {
                    presenceList.Add(new PresenceRecord
                    {
                        EmployeeName = emp.EmployeeName,
                        LoginTime = DateTime.MinValue,
                        LogoutTime = DateTime.MinValue,
                        Status = StatusConstants.Remote
                    });

                    continue;
                }

                // ✅ First session start = Login
                var loginTime = empLogs.First().StartTime;

                // ✅ Last session end = Logout
                var logoutTime = empLogs.Max(l => l.EndTime);

                presenceList.Add(new PresenceRecord
                {
                    EmployeeName = emp.EmployeeName,
                    LoginTime = loginTime,
                    LogoutTime = logoutTime,
                    Status = StatusConstants.Office
                });
            }

            return presenceList;
        }

        public async Task<List<PresenceRecord>> GetAllPresenceAsync()
        {
            return await GetPresenceAsync();
        }

        public async Task<List<PresenceRecord>> GetPresenceByEmployeeNameAsync(string employeeName)
        {
            var all = await GetPresenceAsync();

            return all
                .Where(p => p.EmployeeName.Equals(employeeName, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}