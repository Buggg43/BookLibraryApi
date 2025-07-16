using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public record DeleteBookCommand(int BookId, ClaimsPrincipal User) : IRequest<IResult>;
}
