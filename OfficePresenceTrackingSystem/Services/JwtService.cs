using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace OfficePresenceTrackingSystem.Services
{
    public class JwtService
    {
        private readonly string _key =
            "sdfghjkloiuytrdsdfghjkoiuytrdsdfghjkoiuyt";

        public string GenerateToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.UTF8.GetBytes(_key);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),

                Expires = DateTime.UtcNow.AddHours(2),

                Issuer = "office-app",
                Audience = "office-app",

                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(keyBytes),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}