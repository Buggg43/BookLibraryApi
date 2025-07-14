using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Features.Users.Queries;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Services;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
        private readonly IMediator _mediator;
        public AuthController(PasswordHasher<User> hasher, LibraryDbContext context, IMapper mapper, JwtService token, IMediator mediator)
        {
            _context = context;
            _hasher = hasher;
            _mapper = mapper;
            _token = token;
            _mediator = mediator;
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

            //var isRevoked = await _context.RefreshTokens.FirstOrDefaultAsync(b => b.UserId == user.Id); // test don't mind / remove later

            var token = _token.GenerateToken(user);
            var refresh = _token.GenerateRefreshToken(user);

            var pair = new TokenPairDto
            {
                AccessToken = token,
                RefreshToken = refresh.Token,
            };
            await _context.RefreshTokens.AddAsync(refresh);
            await _context.SaveChangesAsync();

            return Ok(pair);
        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogOutRequestDto logout)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var tokenFromDb = await _context.RefreshTokens.FirstOrDefaultAsync(b => b.Token == logout.refreshToken);
            if (tokenFromDb == null)
                return NotFound("Token nie istnieje");

            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (tokenFromDb.UserId != currentUserId)
                return Forbid();

            if (tokenFromDb.isRevoked == true || tokenFromDb.Expires < DateTime.UtcNow)
                return NoContent();

            tokenFromDb.isRevoked = true;
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [Authorize]
        [HttpPost("logout/all")]
        public async Task<IActionResult> LogoutAll()
        {
            var currentUserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);

            var tokensFromDb = await _context.RefreshTokens
                .Where(t => t.UserId == currentUserId && !t.isRevoked && t.Expires > DateTime.UtcNow)
                .ToListAsync();

            foreach (var token in tokensFromDb)
            {
                token.isRevoked = true;
            }

            await _context.SaveChangesAsync();
            return NoContent();
        }
        [Authorize]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            return Ok("Tylko zalogowany użytkowni to widzi!");
        }
        [Authorize]
        [HttpGet("me")]
        public IActionResult GetCurrentUser()
        {
            var query = new GetCurrentUserQuery();
            var dto = query.Execute(User);
            return Ok(dto);
        }
        [Authorize]
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            var _changePassword = new ChangePasswordCommandHandler(_context,_hasher);
            return await _changePassword.ExecuteAsync(User, changePasswordDto);
        }
        [Authorize]
        [HttpPut("me")]
        public async Task<IResult> UpdateCurrentUser([FromBody] UpdateUserDto updateUserDto)
        {
            var command = new UpdateUserCommand(User,updateUserDto);
            return await _mediator.Send(command);
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

            var refresh = new RefreshToken();
            refresh = _token.GenerateRefreshToken(existingRefreshToken.User);
            var newAccess = _token.GenerateToken(existingRefreshToken.User);


            await _context.RefreshTokens.AddAsync(refresh);
            await _context.SaveChangesAsync();

            var pair = new TokenPairDto 
            { 
                AccessToken = newAccess, 
                RefreshToken = refresh.Token 
            };

            return Ok(pair);
        }
    }
}
