using Microsoft.AspNetCore.Mvc;
using OfficePresenceTrackingSystem.DTOs;
using OfficePresenceTrackingSystem.Models;
using OfficePresenceTrackingSystem.Services;

namespace OfficePresenceTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;

        public AuthController(JwtService jwtService)
        {
            _jwtService = jwtService;
        }

        // TEMP: Hardcoded users (replace with DB later)
        private readonly List<User> users = new()
        {
            new User { Username = "admin", Password = "admin123", Role = "Admin" },
            new User { Username = "emp1", Password = "emp123", Role = "Employee", EmployeeId = "E001" }
        };

        [HttpPost("login")]
        public IActionResult Login(LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Username) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest("Username and Password are required");

            var user = users.FirstOrDefault(u =>
                u.Username.Equals(dto.Username, StringComparison.OrdinalIgnoreCase) &&
                u.Password == dto.Password);

            if (user == null)
                return Unauthorized("Invalid credentials");

            var token = _jwtService.GenerateToken(user);

            return Ok(new { token });
        }
    }
}