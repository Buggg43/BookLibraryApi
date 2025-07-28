using BookLibraryApi.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace BookLibraryApi.Services
{
    public interface IJwtService
    {
        string GenerateToken(User user);
        RefreshToken GenerateRefreshToken(User user);
    }

    public class JwtService : IJwtService
    {
        private readonly IConfiguration _configuration;

        public JwtService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateToken(User user)
        {
            var keyString = _configuration["Jwt:Key"];
            if (string.IsNullOrEmpty(keyString) || keyString.Length < 16)
                throw new Exception("Jwt:Key must be at least 16 characters long.");

            var issuer = _configuration["Jwt:Issuer"] ?? throw new Exception("Jwt:Issuer is not configured.");
            var expireMinutes = int.TryParse(_configuration["Jwt:ExpireMinutes"], out var minutes) ? minutes : 60;

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: issuer,
                claims: claims,
                notBefore: DateTime.UtcNow,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public RefreshToken GenerateRefreshToken(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user), "User cannot be null when generating refresh token.");

            return new RefreshToken
            {
                UserId = user.Id,
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)),
                Expires = DateTime.UtcNow.AddDays(7),
                isRevoked = false,
                User = user
            };
        }

    }
}
