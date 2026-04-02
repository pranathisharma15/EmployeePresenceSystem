namespace OfficePresenceTrackingSystem.DTOs
{
    public class LoginDto   // This class represents the data transfer object for login requests, containing the username and password fields.
    {
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
    }
}