using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Queries
{
    public record GetBookByIdQuery(int BookId, ClaimsPrincipal User) : IRequest<IResult>;
}
