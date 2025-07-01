using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApi.Controllers
{

    [ApiController]
    public class AuthController : Controller
    {
        public async Task Login()
        {

        }
        public async Task HashPassword(User user, string password)
        {
            user.PasswordHash = password;
        }
        private readonly PasswordHasher<User> _hasher;
        private readonly IMapper _mapper;
        private readonly LibraryDbContext _context;
        public AuthController(PasswordHasher<User> hasher, LibraryDbContext context, IMapper mapper)
        {
            _context = context;
            _hasher = hasher;
            _mapper = mapper;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody]RegisterUserDto userDto)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }
            var user = new User { Username = userDto.Username };
            user.PasswordHash =_hasher.HashPassword(user, userDto.Password);


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
            if(!existUsername)
            {
                return Unauthorized("Nieprawidłowe dane logowania");
            }
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Username == userDto.Username);
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, userDto.Password);

            if(result != PasswordVerificationResult.Success)
            {
                return Unauthorized("Nieprawidłowe dane logowania");
            }

            return Ok();
        }

    }
}
