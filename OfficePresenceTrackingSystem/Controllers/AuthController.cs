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

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            // ✅ Demo users
            if (login.Username == "admin" && login.Password == "admin123")
            {
                var token = _jwtService.GenerateToken(login.Username, "Admin");
                return Ok(new { token });
            }

            if (login.Username == "employee" && login.Password == "emp123")
            {
                var token = _jwtService.GenerateToken(login.Username, "Employee");
                return Ok(new { token });
            }

            return Unauthorized("Invalid username or password");
        }
    }
}