using BookLibraryApi.Models.Dtos;
using MediatR;

namespace BookLibraryApi.Features.Users.Commands
{
    public record LoginUserCommand(LoginUserDto dto ) : IRequest<IResult>;
}
