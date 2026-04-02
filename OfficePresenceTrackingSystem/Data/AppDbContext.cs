using Microsoft.EntityFrameworkCore;
using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Data 
{
    public class AppDbContext : DbContext   // This class represents the Entity Framework Core database context for the application. It manages the connection to the database and provides DbSet properties for each entity type (Employee, WifiLog, PresenceRecord) that we want to store in the database. The constructor takes DbContextOptions to configure the context, such as the database provider and connection string.
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Employee> Employees { get; set; }

        public DbSet<WifiLog> WifiLogs { get; set; }

        public DbSet<PresenceRecord> PresenceRecords { get; set; }
    }
}