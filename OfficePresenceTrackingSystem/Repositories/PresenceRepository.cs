using Microsoft.EntityFrameworkCore;
using OfficePresenceTrackingSystem.Data;
using OfficePresenceTrackingSystem.Models;
using OfficePresenceTrackingSystem.Repositories.Interfaces;

namespace OfficePresenceTrackingSystem.Repositories
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

        public async Task SaveWifiLogsAsync(List<WifiLog> logs)
        {
            try
            {
                if (logs == null || !logs.Any())
                    throw new ArgumentException("WiFi logs cannot be empty.");

                using var transaction = await _context.Database.BeginTransactionAsync();

                var oldLogs = await _context.WifiLogs.ToListAsync();
                _context.WifiLogs.RemoveRange(oldLogs);
                await _context.SaveChangesAsync();

                await _context.WifiLogs.AddRangeAsync(logs);
                await _context.SaveChangesAsync();

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

        public async Task SaveEmployeesAsync(List<Employee> employees)
        {
            try
            {
                if (employees == null || !employees.Any())
                    throw new ArgumentException("Employee data cannot be empty.");

                using var transaction = await _context.Database.BeginTransactionAsync();

                var oldEmployees = await _context.Employees.ToListAsync();
                _context.Employees.RemoveRange(oldEmployees);
                await _context.SaveChangesAsync();

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

        public async Task<List<WifiLog>> GetWifiLogsAsync()
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

        public async Task<List<Employee>> GetEmployeesAsync()
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

        public async Task SavePresenceRecordsAsync(List<PresenceRecord> records)
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

        public async Task<List<PresenceRecord>> GetPresenceRecordsAsync()
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