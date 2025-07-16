using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class CreateBookCommandHandler : IRequestHandler<CreateBookCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;

        public CreateBookCommandHandler(LibraryDbContext context, IMapper mapper)
        { 
            _context = context; 
            _mapper = mapper; 
        }
        public async Task<IResult>Handle(CreateBookCommand request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(request.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var book = _mapper.Map<Book>(request.dto);

            book.UserId = userId;
            book.isRead = false;
            book.isFavorite = false;

            await _context.Books.AddAsync(book);
            await _context.SaveChangesAsync();

            return Results.Created($"/api/books/{book.Id}", null);
        }
    }
}
