using BookLibraryApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class LogoutAllCommandHandler : IRequestHandler<LogoutAllCommand, IResult>
    {
        private readonly LibraryDbContext _context;

        public LogoutAllCommandHandler(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<IResult> Handle(LogoutAllCommand request, CancellationToken cancelationToken)
        {
            var userId = int.Parse(request.user.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Id == userId);
            if (user == null)
                return Results.Unauthorized();
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
