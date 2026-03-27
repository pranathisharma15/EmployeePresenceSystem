using System;

namespace OfficePresenceTrackingSystem.Models
{
    public class WifiLog
    {
        public int Id { get; set; }

        public string IPAddress { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string MACAddress { get; set; } = string.Empty;

        public string HostName { get; set; } = string.Empty;
    }
}