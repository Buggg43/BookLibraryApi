using BookLibraryApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        public LogoutCommandHandler(LibraryDbContext context) 
        {
            _context = context;
        }
        public async Task<IResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = request.dto.refreshToken;
            var userId = int.Parse(request.user.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (token == null) 
                return Results.NotFound();

            var tokenLogout = await _context.RefreshTokens.FirstOrDefaultAsync(b => b.Token == token);
            if(tokenLogout == null || tokenLogout.UserId != userId) 
                return Results.Forbid();

            if (tokenLogout.isRevoked || tokenLogout.Expires < DateTime.UtcNow)
                return Results.NoContent();


            tokenLogout.isRevoked = true;

            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
