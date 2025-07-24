using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Features.Users.Queries;
using BookLibraryApi.Models.Dtos;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookLibraryApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IMediator _mediator;
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserDto userDto)
        {
            return (IActionResult)await _mediator.Send(new RegisterUserCommand(userDto));
        }
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserDto userDto)
        {
            return (IActionResult)await _mediator.Send(new LoginUserCommand(userDto)); ;
        }
        [Authorize]
        [HttpPost("Logout")]
        public async Task<IActionResult> Logout([FromBody] LogOutRequestDto logout)
        {
            return (IActionResult)await _mediator.Send(new LogoutCommand(logout, User));
        }
        [Authorize]
        [HttpPost("logout/all")]
        public async Task<IActionResult> LogoutAll()
        {
            return (IActionResult)await _mediator.Send(new LogoutAllCommand(User));
        }
        [Authorize]
        [HttpGet("secure-data")]
        public IActionResult GetSecureData()
        {
            return Ok("Tylko zalogowany użytkowni to widzi!");
        }
        [Authorize]
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUser()
        {
            return (IActionResult)await _mediator.Send(new GetCurrentUserQuery(User));
        }
        [Authorize]
        [HttpPut("me/password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            return (IActionResult)await _mediator.Send(new ChangePasswordCommand(User, changePasswordDto));
        }
        [Authorize]
        [HttpPut("me")]
        public async Task<IActionResult> UpdateCurrentUser([FromBody] UpdateUserDto updateUserDto)
        {
            return (IActionResult)await _mediator.Send(new UpdateUserCommand(User, updateUserDto));
        }
        [Authorize(Roles="Admin")]
        [HttpGet("admin/users")]
        public async Task<IActionResult> GetAllUsers()
        {
            return (IActionResult)await _mediator.Send(new GetAllUsersQuery(User));
        }
        [Authorize(Roles="Admin")]
        [HttpDelete("admin/users/{id}")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            return (IActionResult)await _mediator.Send(new RemoveUserCommand(id));
        }
        [Authorize(Roles="Admin")]
        [HttpPut("admin/users/role")]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRoleDto newRole)
        {
            return (IActionResult)await _mediator.Send(new UpdateUserRoleCommand(newRole, User));
        }
        [HttpPut("refresh")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            return (IActionResult)await _mediator.Send(new RefreshTokenCommand(refreshTokenDto));
        }
    }
}
