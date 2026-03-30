using Microsoft.EntityFrameworkCore;
using OfficePresenceTrackingSystem.Data;
using OfficePresenceTrackingSystem.Models;
using OfficePresenceTrackingSystem.Repositories.Interfaces;

namespace OfficePresenceTrackingSystem.Repositories
{
    public class PresenceRepository : IPresenceRepository
    {
        private readonly AppDbContext _context;

        public PresenceRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task SaveWifiLogsAsync(List<WifiLog> logs)
        {
            if (logs == null || !logs.Any())
                return;

            // ✅ Remove old WiFi logs before new upload
            var oldLogs = await _context.WifiLogs.ToListAsync();
            _context.WifiLogs.RemoveRange(oldLogs);
            await _context.SaveChangesAsync();

            await _context.WifiLogs.AddRangeAsync(logs);
            await _context.SaveChangesAsync();
        }

        public async Task SaveEmployeesAsync(List<Employee> employees)
        {
            if (employees == null || !employees.Any())
                return;

            // ✅ Remove old employee mappings before upload
            var oldEmployees = await _context.Employees.ToListAsync();
            _context.Employees.RemoveRange(oldEmployees);
            await _context.SaveChangesAsync();

            // ✅ Remove duplicates inside uploaded mapping file
            var uniqueEmployees = employees
                .GroupBy(e => new
                {
                    Name = e.EmployeeName.Trim().ToUpper(),
                    Serial = e.SerialNumber.Trim().ToUpper()
                })
                .Select(g => g.First())
                .ToList();

            await _context.Employees.AddRangeAsync(uniqueEmployees);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WifiLog>> GetWifiLogsAsync()
        {
            return await _context.WifiLogs
                .AsNoTracking()
                .OrderBy(x => x.StartTime)
                .ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task SavePresenceRecordsAsync(List<PresenceRecord> records)
        {
            if (records == null || !records.Any())
                return;

            var oldRecords = await _context.PresenceRecords.ToListAsync();
            _context.PresenceRecords.RemoveRange(oldRecords);
            await _context.SaveChangesAsync();

            await _context.PresenceRecords.AddRangeAsync(records);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PresenceRecord>> GetPresenceRecordsAsync()
        {
            return await _context.PresenceRecords
                .AsNoTracking()
                .OrderBy(x => x.EmployeeName)
                .ToListAsync();
        }
    }
}