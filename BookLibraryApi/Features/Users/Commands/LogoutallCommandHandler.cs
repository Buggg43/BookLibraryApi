using BookLibraryApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class LogoutallCommandHandler : IRequestHandler<LogoutAllCommand, IResult>
    {
        private readonly LibraryDbContext _context;

        public LogoutallCommandHandler(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<IResult> Handle(LogoutAllCommand request, CancellationToken cancelationToken)
        {
            var userId = int.Parse(request.user.FindFirst(ClaimTypes.NameIdentifier).Value);

            var tokens = await _context.RefreshTokens.Where(b => b.UserId == userId).ToListAsync();
            foreach(var token in tokens )
            {
                if(token.isRevoked== true || token.Expires < DateTime.UtcNow) continue;
                token.isRevoked = true;
            }

            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
