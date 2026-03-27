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
            if (logs == null || !logs.Any()) return;

            await _context.WifiLogs.AddRangeAsync(logs);
            await _context.SaveChangesAsync();
        }

        public async Task SaveEmployeesAsync(List<Employee> employees)
        {
            if (employees == null || !employees.Any()) return;

            await _context.Employees.AddRangeAsync(employees);
            await _context.SaveChangesAsync();
        }

        public async Task<List<WifiLog>> GetWifiLogsAsync()
        {
            return await _context.WifiLogs.AsNoTracking().ToListAsync();
        }

        public async Task<List<Employee>> GetEmployeesAsync()
        {
            return await _context.Employees.AsNoTracking().ToListAsync();
        }

        public async Task SavePresenceRecordsAsync(List<PresenceRecord> records)
        {
            if (records == null || !records.Any()) return;

            // Clear old records (important to avoid duplicates)
            _context.PresenceRecords.RemoveRange(_context.PresenceRecords);

            await _context.SaveChangesAsync(); // <-- important

            await _context.PresenceRecords.AddRangeAsync(records);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PresenceRecord>> GetPresenceRecordsAsync()
        {
            return await _context.PresenceRecords.AsNoTracking().ToListAsync();
        }
    }
}