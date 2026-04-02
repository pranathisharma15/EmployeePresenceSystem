using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OfficePresenceTrackingSystem.Services
{
    public class JwtService // This service is responsible for generating JWT tokens for authenticated users. It uses the configuration to retrieve the secret key and other token settings, and it logs any errors that occur during token generation.
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtService> _logger;

        public JwtService(
            IConfiguration configuration,
            ILogger<JwtService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public string GenerateToken(string username, string role)   // This method generates a JWT token for a given username and role. It retrieves the secret key from the configuration, creates the necessary claims, and constructs the token descriptor with the appropriate settings. If any errors occur during this process, they are logged and an application exception is thrown.
        {
            try
            {
                var secretKey = _configuration["Jwt:Key"];

                if (string.IsNullOrWhiteSpace(secretKey))
                    throw new Exception("JWT secret key is missing.");

                var keyBytes = Encoding.UTF8.GetBytes(secretKey);

                var claims = new List<Claim>    // Create claims for the token, including the username, role, and a unique identifier (JTI)
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                var tokenDescriptor = new SecurityTokenDescriptor   // Define the token descriptor, which includes the claims, expiration time, issuer, audience, and signing credentials
                {
                    Subject = new ClaimsIdentity(claims),
                    Expires = DateTime.UtcNow.AddHours(2),
                    Issuer = _configuration["Jwt:Issuer"],
                    Audience = _configuration["Jwt:Audience"],
                    SigningCredentials = new SigningCredentials(
                        new SymmetricSecurityKey(keyBytes),
                        SecurityAlgorithms.HmacSha256
                    )
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var token = tokenHandler.CreateToken(tokenDescriptor);

                return tokenHandler.WriteToken(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating JWT token");
                throw new ApplicationException("Failed to generate authentication token.");
            }
        }
    }
}