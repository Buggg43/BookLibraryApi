using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class UpdateBookCommandHandler : IRequestHandler<UpdateBookCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;
        public UpdateBookCommandHandler(LibraryDbContext context, IMapper mapper)
        {
            _context= context;
            _mapper = mapper;
        }
        public async Task<IResult> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(request.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var book = await _context.Books.FirstOrDefaultAsync(b => b.Id == request.BookId);
            if (book == null)
                return Results.NotFound();
            if (book.UserId != userId)
                return Results.Forbid();

            _mapper.Map(request.dto, book);

            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
