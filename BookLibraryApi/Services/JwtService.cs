using BookLibraryApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;
using BookLibraryApi.Models.Dtos;

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
        public RefreshToken GenerateRefreshToken(User user)
        {
            var random = RandomNumberGenerator.GetBytes(32);
            var token = Convert.ToBase64String(random);

            //var expiresMinutes = int.Parse(_configure["ExpireMinutes"]);//to add ExpireDays in appsetting

            var expire = DateTime.UtcNow.AddDays(7); 

            var refreshUser = new RefreshToken();
            
            
            refreshUser.Id = user.Id;
            refreshUser.UserId = user.Id;
            refreshUser.isRevoked = false;// potem do zmiany 
            refreshUser.Expires = expire;
            refreshUser.Token = token;
            refreshUser.User = user;
            

            return refreshUser;
        }
    }
}
