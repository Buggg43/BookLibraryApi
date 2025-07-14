using BookLibraryApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        public LogoutCommandHandler() 
        {

        }
        public async Task<IResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var token = request.dto.refreshToken;
            var userId = int.Parse(request.user.FindFirst(ClaimTypes.NameIdentifier).Value);
            if (token == null ) 
                return Results.NotFound();

            await _context.RefreshTokens.FirstOrDefaultAsync();
            


            return Results.Ok();
        }
    }
}
