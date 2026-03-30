using Microsoft.AspNetCore.Mvc;
using OfficePresenceTrackingSystem.DTOs;
using OfficePresenceTrackingSystem.Services;

namespace OfficePresenceTrackingSystem.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwtService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(
            JwtService jwtService,
            ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginDto login)
        {
            try
            {
                // Null request check
                if (login == null)
                {
                    _logger.LogWarning("Login request body is null");
                    return BadRequest(new
                    {
                        message = "Login request cannot be empty"
                    });
                }

                //  Field validation
                if (string.IsNullOrWhiteSpace(login.Username) ||
                    string.IsNullOrWhiteSpace(login.Password))
                {
                    _logger.LogWarning("Username or password missing");
                    return BadRequest(new
                    {
                        message = "Username and password are required"
                    });
                }

                //  Demo admin login
                if (login.Username == "admin" &&
                    login.Password == "admin123")
                {
                    var token = _jwtService.GenerateToken(
                        login.Username,
                        "Admin");

                    return Ok(new
                    {
                        message = "Login successful",
                        token
                    });
                }

                // Demo employee login
                if (login.Username == "employee" &&
                    login.Password == "emp123")
                {
                    var token = _jwtService.GenerateToken(
                        login.Username,
                        "Employee");

                    return Ok(new
                    {
                        message = "Login successful",
                        token
                    });
                }

                _logger.LogWarning("Invalid login attempt for user {Username}", login.Username);

                return Unauthorized(new
                {
                    message = "Invalid username or password"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during login");

                return StatusCode(500, new
                {
                    message = "An unexpected error occurred while logging in"
                });
            }
        }
    }
}