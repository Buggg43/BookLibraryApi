using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Queries
{
    public class GetBookByIdQueryHandler : IRequestHandler<GetBookByIdQuery, IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;
        public GetBookByIdQueryHandler(LibraryDbContext context, IMapper mapper) 
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IResult>Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(request.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var book = await _context.Books.FirstOrDefaultAsync(book => book.Id == request.BookId);
            if (book.UserId != userId)
                return Results.Forbid();

            var result = _mapper.Map<BookReadDto>(book);

            return Results.Ok(result);
        }
    }
}
