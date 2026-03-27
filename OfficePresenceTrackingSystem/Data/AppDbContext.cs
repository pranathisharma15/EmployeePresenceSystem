using Microsoft.EntityFrameworkCore;
using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<WifiLog> WifiLogs { get; set; }

        public DbSet<PresenceRecord> PresenceRecords { get; set; }
    }
}