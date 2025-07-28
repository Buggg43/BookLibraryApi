using BookLibraryApi.Data;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Services;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace BookLibraryApi.Features.Users.Commands
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly IJwtService _token;
        public RefreshTokenCommandHandler(LibraryDbContext context, IJwtService token) 
        {
            _context = context;
            _token = token;
        }
        public async Task<IResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
        {
            var oldToken = request.dto.token;

            var existingRefreshToken = await _context.RefreshTokens
                .Include(r => r.User)
                .FirstOrDefaultAsync(b => b.Token == oldToken);

            if (existingRefreshToken == null)
                return Results.NotFound();

            if (existingRefreshToken.Expires < DateTime.UtcNow || existingRefreshToken.isRevoked)
                return Results.Unauthorized();

            var user = existingRefreshToken.User;

            if (user == null)
                return Results.Unauthorized(); 

            var newRefreshToken = _token.GenerateRefreshToken(user);
            var newAccessToken = _token.GenerateToken(user);

            await _context.RefreshTokens.AddAsync(newRefreshToken);
            await _context.SaveChangesAsync();

            var pair = new TokenPairDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken.Token
            };

            return Results.Ok(pair);
        }

    }
}
