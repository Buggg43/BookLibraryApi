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
            var userId = int.Parse(request.user.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Id == userId);
            if (user == null)
                return Results.NotFound();

            var role = request.user.FindFirst(ClaimTypes.Role).Value;
            if(role == null || role != "Admin") 
                return Results.Forbid();
            if(role == request.dto.Role)
                return Results.BadRequest();

            user.Role = request.dto.Role;

            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
