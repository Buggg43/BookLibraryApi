using BookLibraryApi.Models.Dtos;
using MediatR;
using System.Security.Claims;
using AutoMapper;
using BookLibraryApi.Data;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApi.Features.Users.Queries
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, IResult>
    {
        private readonly IMapper _mapper;
        private readonly LibraryDbContext _context;
        public GetCurrentUserQueryHandler(IMapper mapper, LibraryDbContext context)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IResult> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(request.user.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == userId);
            if (user == null) 
                return Results.BadRequest();

            var username = request.user.FindFirst(ClaimTypes.Name).Value;
            var role = request.user.FindFirst(ClaimTypes.Role).Value ?? "User";


            return Results.Ok(new UserReadDto
            {
                Id = userId,
                Username = username,
                Role = role,
            });
        }
    }
}
