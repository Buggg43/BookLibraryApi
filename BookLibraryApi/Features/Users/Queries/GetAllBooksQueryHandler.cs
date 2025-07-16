using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApi.Features.Users.Queries
{
    public class GetAllBooksQueryHandler : IRequestHandler<GetAllBooksQuery,IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;
        public GetAllBooksQueryHandler(LibraryDbContext context, IMapper mapper) 
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IResult> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
        {
            var allBooks = await _context.Books.ToListAsync();
            var result = _mapper.Map<List<BookReadDto>>(allBooks);
            return Results.Ok(result);
        }
    }
}
