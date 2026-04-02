namespace OfficePresenceTrackingSystem.Models
{
    public class User   //  This class represents a user in the system, containing properties for username, password, role, and employee ID. The password should be stored as a hash for security reasons, and the role indicates whether the user is an Admin or an Employee. The EmployeeId links the user to their corresponding employee record in the system.
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty; // hash later

        public string Role { get; set; } = string.Empty; // Admin / Employee

        public string EmployeeId { get; set; } = string.Empty;
    }
}