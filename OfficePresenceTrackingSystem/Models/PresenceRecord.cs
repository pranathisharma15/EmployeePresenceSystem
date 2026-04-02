using System;

namespace OfficePresenceTrackingSystem.Models
{
    public class PresenceRecord // This class represents a record of an employee's presence in the office. It contains properties for the employee's name, device serial number, department, team, and designation. This model is used to store and retrieve presence information in the database and to return presence data through the API.
    {
        public int Id { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Team { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;

        public DateTime LoginTime { get; set; }

        public DateTime? LogoutTime { get; set; }

        public string Status { get; set; } = string.Empty;

        public double DurationHours =>
            LogoutTime.HasValue
                ? Math.Round((LogoutTime.Value - LoginTime).TotalHours, 2)
                : 0;
    }
}