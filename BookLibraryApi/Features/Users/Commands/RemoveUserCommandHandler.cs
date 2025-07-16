using BookLibraryApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApi.Features.Users.Commands
{
    public class RemoveUserCommandHandler :  IRequestHandler<RemoveUserCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        public RemoveUserCommandHandler(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<IResult> Handle(RemoveUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == request.id);
            if (user == null)
                return Results.NotFound();
            if (user.Role == "Admin")
                return Results.Forbid();

            var userBooks = await _context.Books.Where(b => b.UserId == request.id).ToListAsync();
            _context.Books.RemoveRange(userBooks);


            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
