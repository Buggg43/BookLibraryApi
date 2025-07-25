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

            var filters = request.dto;
            var query = _context.Books.AsQueryable();

            if (filters.isRead.HasValue)
                query = query.Where(b => b.isRead == filters.isRead.Value);

            if (filters.isFavorite.HasValue)
                query = query.Where(b => b.isFavorite == filters.isFavorite.Value);

            if (!string.IsNullOrEmpty(filters.Title))
                query = query.Where(b => b.Title.Contains(filters.Title));

            if (!string.IsNullOrEmpty(filters.Author))
                query = query.Where(b => b.Author.Contains(filters.Author));

            if (filters.Year.HasValue)
                query = query.Where(b => b.Year.Year == filters.Year.Value.Year);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / filters.PageSize);

            var books = await query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToListAsync();

            var filterResult = _mapper.Map<List<BookReadDto>>(books);

            return Results.Ok(new GetUserBooksResponseDto
            {
                Page = filters.Page,
                PageSize = filters.PageSize,
                TotalItems = totalItems,
                TotalPages = totalPages,
                Data = filterResult
            });
        }
    }
}
