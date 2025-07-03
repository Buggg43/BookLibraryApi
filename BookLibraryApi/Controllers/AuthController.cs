using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using System.Security.Claims;

namespace BookLibraryApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        public async Task HashPassword(User user, string password)
        {
            user.PasswordHash = password;
        }
        private readonly PasswordHasher<User> _hasher;
        private readonly IMapper _mapper;
        private readonly LibraryDbContext _context;
        private readonly JwtService _token;
        public AuthController(PasswordHasher<User> hasher, LibraryDbContext context, IMapper mapper, JwtService token)
        {
            _context = context;
            _hasher = hasher;
            _mapper = mapper;
            _token = token;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = new User { Username = userDto.Username, Role = "User" };
            user.PasswordHash = _hasher.HashPassword(user, userDto.Password);


            var exist = await _context.Users.AnyAsync(b => b.Username == user.Username);
            if (exist)
            {
                return Conflict("Użytkownik już istnieje");
            }

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Created();
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var existUsername = await _context.Users.AnyAsync(b => b.Username == userDto.Username);
            if (!existUsername)
            {
                return Unauthorized("Nieprawidłowe dane logowania");
            }
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Username == userDto.Username);
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);

            if (result != PasswordVerificationResult.Success)
            {
                return Unauthorized("Nieprawidłowe dane logowania");
            }
            var token = _token.GenerateToken(user);

            return Ok(new { token });
        }
        [Authorize]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            return Ok("Tylko zalogowany użytkowni to widzi!");
        }
        [Authorize]
        [HttpGet("me")]
        public IActionResult WhoIsLogged()
        {
            if (!ModelState.IsValid) {
                return BadRequest();
            }
            if (!HttpContext.User.Identity.IsAuthenticated)
                return Unauthorized();


            var id = int.Parse(HttpContext.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var username = HttpContext.User.FindFirst(ClaimTypes.Name)?.Value;

            var me = new UserReadDto { Id = id, Username = username };

            return Ok(me);
        }
        [Authorize(Roles="Admin")]
        [HttpGet("admin/users")]
        public async Task<IActionResult> GetAllusers()
        {
            if (!ModelState.IsValid)
                return BadRequest();
            if (!HttpContext.User.Identity.IsAuthenticated)
                return Forbid();
            var users = _context.Users.ToListAsync();
            return Ok(_mapper.Map<List<UserReadDto>>(users));
        }
        [Authorize(Roles="Admin")]
        [HttpDelete("admin/users/{id}")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            if (!ModelState.IsValid)
                return BadRequest();
            var user = _context.Users.FirstOrDefault(b => b.Id == id);
            if (user == null)
                return BadRequest();
            if (user.Role == "Admin")
                return Forbid("User role is to high");
            var userBooks = await _context.Books.Where(b => b.UserId == id).ToListAsync();

            _context.Books.RemoveRange(userBooks);
            _context.Users.Remove(user);

            _context.SaveChanges();

            return Ok();
        }
        [Authorize(Roles="Admin")]
        [HttpPut("admin/users/{id}/role")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UpdateUserRoleDto newRole)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _context.Users.FirstOrDefaultAsync(a => a.Id == id);

            if (user == null)
                return NotFound();

            user.Role = newRole.Role;

            _context.SaveChanges();

            return Ok($"User{user.Username} has now role set to: {user.Role}");
        }
        [HttpPut("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var existingRefreshToken = await _context.RefreshTokens.FirstOrDefaultAsync(a => a.Token == refreshTokenDto.token);
            if (existingRefreshToken == null)
                return NotFound();
            if (existingRefreshToken.isRevoked == true || existingRefreshToken.Expires < DateTime.UtcNow)
                return Forbid();

            existingRefreshToken.isRevoked = true;

            var refreshUser = new RefreshToken();
            refreshUser = _token.GenerateRefreshToken(existingRefreshToken.User);
            var newAccess = _token.GenerateToken(existingRefreshToken.User);


            await _context.RefreshTokens.AddAsync(refreshUser);
            await _context.SaveChangesAsync();

            return Ok(new TokenPairDto { AccessToken = newAccess, RefreshToken = refreshUser.Token });
        }
    }
}
