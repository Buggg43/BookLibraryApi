using BookLibraryApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

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
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Id == request.dto.UserId);
            if (user == null)
                return Results.NotFound();

            user.Role = request.dto.Role;

            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
