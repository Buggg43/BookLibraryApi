using BookLibraryApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class UpdateUserRoleCommandHandler :  IRequestHandler<UpdateUserRoleCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        public UpdateUserRoleCommandHandler(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<IResult> Handle(UpdateUserRoleCommand request, CancellationToken cancelationToken)
        {
            var requesterId = int.Parse(request.user.FindFirst(ClaimTypes.NameIdentifier).Value);
            var targetUser = await _context.Users.FirstOrDefaultAsync(b => b.Id == request.dto.UserId);
            if (targetUser == null)
                return Results.NotFound();

            var requesterRole = request.user.FindFirst(ClaimTypes.Role).Value.ToString();
            if (requesterRole != "Admin" || requesterId == request.dto.UserId)
                return Results.Forbid();

            if (targetUser.Role == request.dto.Role || request.dto.Role == null)
                return Results.BadRequest();

            targetUser.Role = request.dto.Role;

            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
