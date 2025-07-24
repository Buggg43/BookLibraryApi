using BookLibraryApi.Data;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApi.Features.Users.Commands
{
    public class LoginUserCommandHandler : IRequestHandler<LoginUserCommand,IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly PasswordHasher<User> _hasher;
        private readonly IJwtService _token;
        public LoginUserCommandHandler(LibraryDbContext context, PasswordHasher<User> hasher, IJwtService token) 
        {
            _context = context;
            _hasher = hasher;
            _token = token;
        }
        public async Task<IResult> Handle(LoginUserCommand request, CancellationToken cancellationToken )
        {
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Username == request.dto.Username);
            if (user == null)
                return Results.Unauthorized();
            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.dto.Password);
            if(result != PasswordVerificationResult.Success)
                return Results.Unauthorized();
            var token = _token.GenerateToken(user);
            var refresh = _token.GenerateRefreshToken(user);


            if (token == null || refresh == null)
                return Results.Unauthorized();
            var pair = new TokenPairDto
            {
                AccessToken = token,
                RefreshToken = refresh.Token
            };

            await _context.RefreshTokens.AddAsync(refresh);
            await _context.SaveChangesAsync();
            
            return Results.Ok(pair);
        }
    }
}
