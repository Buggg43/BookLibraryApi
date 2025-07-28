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
        public async Task<IResult> Register([FromBody] RegisterUserDto userDto)
        {
            return await _mediator.Send(new RegisterUserCommand(userDto));
        }

        [HttpPost("login")]
        public async Task<IResult> Login([FromBody] LoginUserDto userDto)
        {
            return await _mediator.Send(new LoginUserCommand(userDto));
        }

        [Authorize]
        [HttpPost("logout")]
        public async Task<IResult> Logout([FromBody] LogOutRequestDto logout)
        {
            return await _mediator.Send(new LogoutCommand(logout, User));
        }

        [Authorize]
        [HttpPost("logout/all")]
        public async Task<IResult> LogoutAll()
        {
            return await _mediator.Send(new LogoutAllCommand(User));
        }

        [Authorize]
        [HttpGet("secure-data")]
        public IResult GetSecureData()
        {
            return Results.Ok("Tylko zalogowany użytkownik to widzi!");
        }

        [Authorize]
        [HttpGet("me")]
        public async Task<IResult> GetCurrentUser()
        {
            return await _mediator.Send(new GetCurrentUserQuery(User));
        }

        [Authorize]
        [HttpPut("me/password")]
        public async Task<IResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto)
        {
            return await _mediator.Send(new ChangePasswordCommand(User, changePasswordDto));
        }

        [Authorize]
        [HttpPut("me")]
        public async Task<IResult> UpdateCurrentUser([FromBody] UpdateUserDto updateUserDto)
        {
            return await _mediator.Send(new UpdateUserCommand(User, updateUserDto));
        }

        [Authorize(Roles = "Admin")]
        [HttpGet("admin/users")]
        public async Task<IResult> GetAllUsers()
        {
            return await _mediator.Send(new GetAllUsersQuery(User));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("admin/users/{id}")]
        public async Task<IResult> RemoveUser(int id)
        {
            return await _mediator.Send(new RemoveUserCommand(id));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("admin/users/role")]
        public async Task<IResult> UpdateUser([FromBody] UpdateUserRoleDto newRole)
        {
            return await _mediator.Send(new UpdateUserRoleCommand(newRole, User));
        }

        [HttpPut("refresh")]
        public async Task<IResult> RefreshToken([FromBody] RefreshTokenRequestDto refreshTokenDto)
        {
            return await _mediator.Send(new RefreshTokenCommand(refreshTokenDto));
        }
    }
}
