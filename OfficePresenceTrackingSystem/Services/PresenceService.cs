using OfficePresenceTrackingSystem.Constants;
using OfficePresenceTrackingSystem.Helpers;
using OfficePresenceTrackingSystem.Models;
using OfficePresenceTrackingSystem.Repositories.Interfaces;
using OfficePresenceTrackingSystem.Services.Interfaces;

namespace OfficePresenceTrackingSystem.Services
{
    public class PresenceService : IPresenceService // This service implements the IPresenceService interface and contains the business logic for handling presence-related operations, such as uploading WiFi logs and employee mappings, and generating presence records based on the uploaded data. It interacts with the repository layer to persist and retrieve data from the database, and uses logging to track actions and handle errors effectively.
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

        public async Task UploadWifiLogsAsync(IFormFile file)   // This method handles the uploading of WiFi log files. It validates the file, parses the logs using a helper class, and saves the logs to the database through the repository. It also includes error handling to log any issues that occur during the process and throws an exception if the upload fails.
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

        public async Task UploadMappingsAsync(IFormFile file)   // This method handles the uploading of employee mapping files. It validates the file, parses the employee data using a helper class, and saves the employee data to the database through the repository. It also includes error handling to log any issues that occur during the process and throws an exception if the upload fails.
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

        public async Task<List<PresenceRecord>> GetPresenceAsync(DateTime? date = null) // This method generates presence records based on the uploaded WiFi logs and employee mappings. It retrieves the logs and employee data from the repository, processes the data to determine the presence status of each employee, and saves the generated presence records back to the database. It also includes error handling to log any issues that occur during the process and throws an exception if the generation of presence records fails.
        {
            try
            {
                var logs = await _repository.GetWifiLogsAsync();
                var employees = await _repository.GetEmployeesAsync();

                if (!employees.Any())
                    return new List<PresenceRecord>();

                var uniqueEmployees = employees // This code processes the employee data to ensure that only unique entries are considered for generating presence records. It filters out any entries that have missing or whitespace-only employee names or serial numbers, and then groups the remaining entries by their serial numbers (after trimming and converting to uppercase) to ensure uniqueness. Finally, it selects the first entry from each group to create a list of unique employees that will be used for presence record generation.
                    .Where(e => !string.IsNullOrWhiteSpace(e.EmployeeName)
                             && !string.IsNullOrWhiteSpace(e.SerialNumber))
                    .GroupBy(e => e.SerialNumber.Trim().ToUpper())
                    .Select(g => g.First())
                    .ToList();

                var filteredLogs = logs
                .Where(l => !string.IsNullOrWhiteSpace(l.HostName))
                 .Where(l => uniqueEmployees.Any(e =>
                   e.SerialNumber.Trim().Equals(
                       l.HostName.Trim(),
                        StringComparison.OrdinalIgnoreCase)))
                .ToList();

                var logLookup = filteredLogs
                    .GroupBy(l => l.HostName.Trim().ToUpper())
                    .ToDictionary(g => g.Key, g => g.OrderBy(x => x.StartTime).ToList());

                var presenceList = new List<PresenceRecord>();

                foreach (var emp in uniqueEmployees)
                {
                    var serial = emp.SerialNumber.Trim().ToUpper();

                    if (!logLookup.ContainsKey(serial)) // If there are no logs for the employee's serial number, we assume they are working remotely. In this case, we create a presence record with the login time set to the current date and the logout time set to null, indicating that the employee is currently working remotely. The status is set to "Remote" to reflect their remote work status.
                    {
                        presenceList.Add(new PresenceRecord
                        {
                            EmployeeName = emp.EmployeeName,
                            Department = emp.Department,
                            Team = emp.Team,
                            Designation = emp.Designation,
                            LoginTime = DateTime.Today,
                            LogoutTime = null,
                            Status = StatusConstants.Remote
                        });

                        continue;
                    }

                    var empLogs = logLookup[serial];

                    presenceList.Add(new PresenceRecord // If there are logs for the employee's serial number, we assume they are working in the office. In this case, we create a presence record with the login time set to the start time of the first log entry and the logout time set to the end time of the last log entry. The status is set to "Office" to reflect their in-office work status.
                    {
                        EmployeeName = emp.EmployeeName,
                        Department = emp.Department,
                        Team = emp.Team,
                        Designation = emp.Designation,
                        LoginTime = empLogs.First().StartTime,
                        LogoutTime = empLogs.Last().EndTime,
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

        public async Task<List<PresenceRecord>> GetAllPresenceAsync()   // This method retrieves all presence records from the database through the repository. It includes error handling to log any issues that occur during the retrieval process and throws an exception if fetching the presence records fails.
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

        public async Task<List<PresenceRecord>> GetPresenceByEmployeeNameAsync(string employeeName) // This method retrieves presence records for a specific employee based on their name. It first checks if the provided employee name is null or whitespace and returns an empty list if it is. Then, it fetches all presence records from the repository and filters them to include only those that match the provided employee name (ignoring case). The filtered list of presence records is returned. The method also includes error handling to log any issues that occur during the retrieval process and throws an exception if fetching the presence records fails.
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

        private void ValidateFile(IFormFile file)   // This private method validates the uploaded file to ensure that it is not null, not empty, and has a .csv extension. If any of these conditions are not met, it throws an ArgumentException with an appropriate message. This validation helps to ensure that only valid CSV files are processed by the service, preventing potential errors during file parsing and data processing.
        {
            if (file == null || file.Length == 0)
                throw new ArgumentException("Uploaded file is empty.");

            if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                throw new ArgumentException("Only CSV files are allowed.");
        }
    }
}