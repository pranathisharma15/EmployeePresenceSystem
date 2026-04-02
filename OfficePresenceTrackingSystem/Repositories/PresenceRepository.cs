using Microsoft.EntityFrameworkCore;
using OfficePresenceTrackingSystem.Data;
using OfficePresenceTrackingSystem.Models;
using OfficePresenceTrackingSystem.Repositories.Interfaces;

namespace OfficePresenceTrackingSystem.Repositories // This class implements the IPresenceRepository interface and provides methods for managing the storage and retrieval of WiFi logs, employee data, and presence records in the application. It uses Entity Framework Core to interact with the database and includes error handling to ensure that any issues during database operations are logged and appropriately handled. The repository pattern helps to abstract the data access layer, making it easier to maintain and test the application.
{
    public class PresenceRepository : IPresenceRepository   
    {
        private readonly AppDbContext _context;
        private readonly ILogger<PresenceRepository> _logger;

        public PresenceRepository(
            AppDbContext context,
            ILogger<PresenceRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task SaveWifiLogsAsync(List<WifiLog> logs) // This method saves a list of WiFi logs to the database. It first checks if the logs are null or empty and throws an exception if they are. Then, it uses a database transaction to ensure that the operation is atomic. It removes any existing WiFi logs from the database before adding the new logs. If any database errors occur during this process, they are logged and an exception is thrown to indicate the failure.
        {
            try     
            {
                if (logs == null || !logs.Any())
                    throw new ArgumentException("WiFi logs cannot be empty.");

                using var transaction = await _context.Database.BeginTransactionAsync();    // Start a database transaction

                // Fetch existing unique keys
                var existingKeys = (await _context.WifiLogs
                .Select(x => $"{x.MACAddress}_{x.StartTime}_{x.EndTime}_{x.IPAddress}")
                .ToListAsync())
                .ToHashSet();

                // Keep only new unique logs
                var newLogs = logs
                    .Where(log => !existingKeys.Contains(
                        $"{log.MACAddress}_{log.StartTime}_{log.EndTime}_{log.IPAddress}"))
                    .ToList();

                if (newLogs.Any())
                {
                    await _context.WifiLogs.AddRangeAsync(newLogs);
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving WiFi logs");
                throw new Exception("Failed to save WiFi logs.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving WiFi logs");
                throw;
            }
        }

        public async Task SaveEmployeesAsync(List<Employee> employees)  // This method saves a list of employee data to the database. It first checks if the employee list is null or empty and throws an exception if it is. Then, it uses a database transaction to ensure that the operation is atomic. It removes any existing employee data from the database before adding the new employee data. The method also ensures that only unique employees with valid names and serial numbers are added to the database. If any database errors occur during this process, they are logged and an exception is thrown to indicate the failure.
        {
            try
            {
                if (employees == null || !employees.Any())
                    throw new ArgumentException("Employee data cannot be empty.");

                using var transaction = await _context.Database.BeginTransactionAsync();

                var oldEmployees = await _context.Employees.ToListAsync();
                _context.Employees.RemoveRange(oldEmployees);
                await _context.SaveChangesAsync();

                var uniqueEmployees = employees // Filter out employees with empty names or serial numbers, and ensure uniqueness based on trimmed and uppercased values
                    .Where(e => !string.IsNullOrWhiteSpace(e.EmployeeName)
                             && !string.IsNullOrWhiteSpace(e.SerialNumber))
                    .GroupBy(e => new
                    {
                        Name = e.EmployeeName.Trim().ToUpper(),
                        Serial = e.SerialNumber.Trim().ToUpper()
                    })
                    .Select(g => g.First())
                    .ToList();

                await _context.Employees.AddRangeAsync(uniqueEmployees);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving employees");
                throw new Exception("Failed to save employee mappings.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving employees");
                throw;
            }
        }

        public async Task<List<WifiLog>> GetWifiLogsAsync() // This method retrieves a list of WiFi logs from the database. It performs an asynchronous operation to fetch the data and returns a list of WifiLog objects. The logs are ordered by their start time for easier analysis. If any errors occur during the database operation, they are logged and an exception is thrown to indicate the failure.
        {
            try
            {
                return await _context.WifiLogs
                    .AsNoTracking()
                    .OrderBy(x => x.StartTime)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching WiFi logs");
                throw;
            }
        }

        public async Task<List<Employee>> GetEmployeesAsync()   // This method retrieves a list of employee data from the database. It performs an asynchronous operation to fetch the data and returns a list of Employee objects. The method uses AsNoTracking() to improve performance since the retrieved entities are not being modified. If any errors occur during the database operation, they are logged and an exception is thrown to indicate the failure.
        {
            try
            {
                return await _context.Employees
                    .AsNoTracking()
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching employees");
                throw;
            }
        }

        public async Task SavePresenceRecordsAsync(List<PresenceRecord> records)    // This method saves a list of presence records to the database. It first checks if the presence records list is null or empty and throws an exception if it is. Then, it uses a database transaction to ensure that the operation is atomic. It removes any existing presence records from the database before adding the new presence records. The new records are ordered by employee name for easier retrieval. If any database errors occur during this process, they are logged and an exception is thrown to indicate the failure.
        {
            try
            {
                if (records == null || !records.Any())
                    throw new ArgumentException("Presence records cannot be empty.");

                using var transaction = await _context.Database.BeginTransactionAsync();

                var oldRecords = await _context.PresenceRecords.ToListAsync();
                _context.PresenceRecords.RemoveRange(oldRecords);
                await _context.SaveChangesAsync();

                await _context.PresenceRecords.AddRangeAsync(records);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error while saving presence records");
                throw new Exception("Failed to save presence records.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while saving presence records");
                throw;
            }
        }

        public async Task<List<PresenceRecord>> GetPresenceAsync(DateTime? date)
        {
            var query = _context.PresenceRecords.AsQueryable();

            if (date.HasValue)
            {
                query = query.Where(x => x.LoginTime.Date == date.Value.Date);
            }

            return await query.ToListAsync();
        }
        public async Task<List<PresenceRecord>> GetPresenceRecordsAsync()   // This method retrieves a list of presence records from the database. It performs an asynchronous operation to fetch the data and returns a list of PresenceRecord objects. The records are ordered by employee name for easier analysis. If any errors occur during the database operation, they are logged and an exception is thrown to indicate the failure.
        {
            try
            {
                return await _context.PresenceRecords
                    .AsNoTracking()
                    .OrderBy(x => x.EmployeeName)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching presence records");
                throw;
            }
        }
    }
}