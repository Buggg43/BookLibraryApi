using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public record LogoutAllCommand(ClaimsPrincipal user) : IRequest<IResult>;

}
