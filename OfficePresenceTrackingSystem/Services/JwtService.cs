using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using OfficePresenceTrackingSystem.Models;

namespace OfficePresenceTrackingSystem.Services
{
    public class JwtService
    {
        private readonly string _key = "sdfghjkloiuytrdsdfghjkoiuytrdsdfghjkoiuyt";

        public string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: "office-app",
                audience: "office-app",
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2), // ✅ better than Now
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}