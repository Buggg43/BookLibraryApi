using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Queries
{
    public record GetAllUsersQuery(ClaimsPrincipal user) : IRequest<IResult>;
}
