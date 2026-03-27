namespace OfficePresenceTrackingSystem.Models
{
    public class User
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty; // hash later

        public string Role { get; set; } = string.Empty; // Admin / Employee

        public string EmployeeId { get; set; } = string.Empty;
    }
}