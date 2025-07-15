using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Queries
{
    public class GetUserBooksQueryHandler : IRequestHandler<GetUserBooksQuery, IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;
        public GetUserBooksQueryHandler(LibraryDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IResult> Handle(GetUserBooksQuery request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(request.user.FindFirst(ClaimTypes.NameIdentifier).Value);
            var filters = request.dto;
            var query = _context.Books.Where(b => b.UserId == userId);

            if (filters.isRead.HasValue)
                query = query.Where(b => b.UserId == userId);
            if(filters.isFavorite.HasValue)
                query = query.Where(b => b.UserId == userId);
            if (filters.Title != null)
                query = query.Where(b => b.UserId == userId);
            if(filters.Author != null)
                query = query.Where(b => b.UserId == userId);
            if(filters.Year != null)
                query = query.Where(b => b.UserId == userId);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling((double)totalItems / filters.PageSize);

            var books = query
                .Skip((filters.Page - 1) * filters.PageSize)
                .Take(filters.PageSize)
                .ToList();

            var filterResult = _mapper.Map<List<BookReadDto>>(books);

            return Results.Ok(new
            {
                filters.Page,
                filters.PageSize,
                totalItems,
                totalPages,
                data = filterResult
            });
        }
    }
}
