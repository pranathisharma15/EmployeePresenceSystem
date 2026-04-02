using Microsoft.AspNetCore.Mvc;
using OfficePresenceTrackingSystem.DTOs;
using OfficePresenceTrackingSystem.Services;

namespace OfficePresenceTrackingSystem.Controllers
{
    [ApiController] // This attribute indicates that this class is an API controller, which provides automatic model validation and other features.
    [Route("api/auth")] // This sets the base route for all actions in this controller to "api/auth". For example, the login action will be accessible at "api/auth/login".
    public class AuthController : ControllerBase    
    {
        private readonly JwtService _jwtService;    // This service is responsible for generating JWT tokens for authenticated users.
        private readonly ILogger<AuthController> _logger;   

        public AuthController(  // The constructor takes in the JwtService and a logger for the AuthController.
            JwtService jwtService,
            ILogger<AuthController> logger)
        {
            _jwtService = jwtService;
            _logger = logger;
        }

        [HttpPost("login")] // This attribute indicates that this action responds to HTTP POST requests at the "login" route, making the full route "api/auth/login".
        public IActionResult Login([FromBody] LoginDto login)   // This action takes a LoginDto object from the request body, which contains the username and password for authentication.
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