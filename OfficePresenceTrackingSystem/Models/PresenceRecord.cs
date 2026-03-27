using System;

namespace OfficePresenceTrackingSystem.Models
{
    public class PresenceRecord
    {
        public int Id { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public DateTime LoginTime { get; set; }

        public DateTime LogoutTime { get; set; }

        public string Status { get; set; } = string.Empty;
    }
}