namespace OfficePresenceTrackingSystem.Models
{
    public class Employee   // This class represents an employee in the office presence tracking system. It contains properties for the employee's ID, name, device serial number, department, team, and designation. This model is used to store and manage employee information in the database and to associate WiFi logs with specific employees.
    {
        public int Id { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string SerialNumber { get; set; } = string.Empty;

        public string Department { get; set; } = string.Empty;

        public string Team { get; set; } = string.Empty;

        public string Designation { get; set; } = string.Empty;
    }
}