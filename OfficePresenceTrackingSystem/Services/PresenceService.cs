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

            var uniqueEmployees = employees
                .GroupBy(e => new
                {
                    Name = e.EmployeeName.Trim().ToUpper(),
                    Serial = e.SerialNumber.Trim().ToUpper()
                })
                .Select(g => g.First())
                .ToList();

            var presenceList = new List<PresenceRecord>();

            foreach (var emp in uniqueEmployees)
            {
                var empLogs = logs
                    .Where(l =>
                        l.HostName.Trim().ToUpper() ==
                        emp.SerialNumber.Trim().ToUpper())
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

                var loginTime = empLogs.First().StartTime;
                var logoutTime = empLogs.Last().StartTime;

                presenceList.Add(new PresenceRecord
                {
                    EmployeeName = emp.EmployeeName,
                    LoginTime = loginTime,
                    LogoutTime = logoutTime,
                    Status = StatusConstants.Office
                });
            }

            // ✅ SAVE TO DB SO SQLITE AUTO-GENERATES ID
            await _repository.SavePresenceRecordsAsync(presenceList);

            // ✅ FETCH SAVED RECORDS WITH GENERATED IDS
            return await _repository.GetPresenceRecordsAsync();
        }

        public async Task<List<PresenceRecord>> GetAllPresenceAsync()
        {
            return await GetPresenceAsync();
        }

        public async Task<List<PresenceRecord>> GetPresenceByEmployeeNameAsync(string employeeName)
        {
            var all = await GetPresenceAsync();

            return all
                .Where(p => p.EmployeeName.Equals(
                    employeeName,
                    StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}