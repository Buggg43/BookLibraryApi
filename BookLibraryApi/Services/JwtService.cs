using BookLibraryApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace BookLibraryApi.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configure;
        public JwtService(IConfiguration configure)
        {
            _configure = configure;
        }
        public string GenerateToken(User user)
        {
            var keyString = _configure["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var epiresMinutes = int.Parse(_configure["Jwt:ExpireMinutes"]);
            var expire = DateTime.Now.AddMinutes(epiresMinutes);

            var token = new JwtSecurityToken(_configure["Jwt:Issuer"], _configure["Jwt:Issuer"], claims, DateTime.UtcNow, expire,credentials);

            var handler = new JwtSecurityTokenHandler();

            var jwt = handler.WriteToken(token);

            return jwt;

        }
    }
}
