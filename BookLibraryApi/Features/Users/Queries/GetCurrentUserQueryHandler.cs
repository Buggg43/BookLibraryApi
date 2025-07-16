using BookLibraryApi.Models.Dtos;
using MediatR;
using System.Security.Claims;
using AutoMapper;

namespace BookLibraryApi.Features.Users.Queries
{
    public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, IResult>
    {
        private readonly IMapper _mapper;
        public GetCurrentUserQueryHandler(IMapper mapper)
        {
            _mapper = mapper;
        }
        public async Task<IResult> Handle(GetCurrentUserQuery request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(request.user.FindFirst(ClaimTypes.NameIdentifier).Value);
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
