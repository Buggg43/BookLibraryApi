using MediatR;

namespace BookLibraryApi.Features.Users.Commands
{
    public record RemoveUserCommand (int id) : IRequest<IResult>;
}
