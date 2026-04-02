using System;

namespace OfficePresenceTrackingSystem.Models
{
    public class WifiLog    // This class represents a WiFi log entry, which contains information about an employee's connection to the office WiFi network. It includes properties for the log ID, IP address, connection start and end times, MAC address, and host name. This model is used to store and manage WiFi log data in the application.
    {
        public int Id { get; set; }

        public string IPAddress { get; set; } = string.Empty;

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        public string MACAddress { get; set; } = string.Empty;

        public string HostName { get; set; } = string.Empty;
    }
}