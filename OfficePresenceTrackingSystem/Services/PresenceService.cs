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
        private readonly ILogger<PresenceService> _logger;

        public PresenceService(
            IPresenceRepository repository,
            ILogger<PresenceService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task UploadWifiLogsAsync(IFormFile file)
        {
            try
            {
                ValidateFile(file);

                using var stream = file.OpenReadStream();
                var logs = CsvParser.ParseWifiLogs(stream);

                await _repository.SaveWifiLogsAsync(logs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading WiFi logs");
                throw new ApplicationException("Failed to upload WiFi logs.");
            }
        }

        public async Task UploadMappingsAsync(IFormFile file)
        {
            try
            {
                ValidateFile(file);

                using var stream = file.OpenReadStream();
                var employees = CsvParser.ParseMappings(stream);

                await _repository.SaveEmployeesAsync(employees);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading employee mappings");
                throw new ApplicationException("Failed to upload employee mappings.");
            }
        }

        public async Task<List<PresenceRecord>> GetPresenceAsync()
        {
            try
            {
                var logs = await _repository.GetWifiLogsAsync();
                var employees = await _repository.GetEmployeesAsync();

                if (!employees.Any())
                    return new List<PresenceRecord>();

                var uniqueEmployees = employees
                    .Where(e => !string.IsNullOrWhiteSpace(e.EmployeeName)
                             && !string.IsNullOrWhiteSpace(e.SerialNumber))
                    .GroupBy(e => new
                    {
                        Name = e.EmployeeName.Trim().ToUpper(),
                        Serial = e.SerialNumber.Trim().ToUpper()
                    })
                    .Select(g => g.First())
                    .ToList();

                var logLookup = logs
                    .Where(l => !string.IsNullOrWhiteSpace(l.HostName))
                    .GroupBy(l => l.HostName.Trim().ToUpper())
                    .ToDictionary(g => g.Key, g => g.OrderBy(x => x.StartTime).ToList());

                var presenceList = new List<PresenceRecord>();

                foreach (var emp in uniqueEmployees)
                {
                    var serial = emp.SerialNumber.Trim().ToUpper();

                    if (!logLookup.ContainsKey(serial))
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

                    var empLogs = logLookup[serial];

                    presenceList.Add(new PresenceRecord
                    {
                        EmployeeName = emp.EmployeeName,
                        LoginTime = empLogs.First().StartTime,
                        LogoutTime = empLogs.Last().StartTime,
                        Status = StatusConstants.Office
                    });
                }

                await _repository.SavePresenceRecordsAsync(presenceList);

                return await _repository.GetPresenceRecordsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating presence records");
                throw new ApplicationException("Failed to calculate presence.");
            }
        }

        public async Task<List<PresenceRecord>> GetAllPresenceAsync()
        {
            try
            {
                return await _repository.GetPresenceRecordsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all presence");
                throw;
            }
        }

        public async Task<List<PresenceRecord>> GetPresenceByEmployeeNameAsync(string employeeName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(employeeName))
                    return new List<PresenceRecord>();

                var all = await _repository.GetPresenceRecordsAsync();

                return all
                    .Where(p => p.EmployeeName.Equals(
                        employeeName,
                        StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error filtering presence by employee name");
                throw;
            }
        }

        private void ValidateFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Uploaded file is empty.");

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only CSV files are allowed.");
        }
    }
}