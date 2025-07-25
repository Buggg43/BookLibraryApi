using BookLibraryApi.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class DeleteBookCommandHandler : IRequestHandler<DeleteBookCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        public DeleteBookCommandHandler(LibraryDbContext context)
        {
            _context = context;
        }
        public async Task<IResult> Handle(DeleteBookCommand request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(request.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var bookToDelete = await _context.Books.FirstOrDefaultAsync(x => x.Id == request.BookId);
            if (bookToDelete == null)
                return Results.NotFound();
            if (userId != bookToDelete.UserId)
                return Results.Forbid();

            _context.Books.Remove(bookToDelete);
            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
