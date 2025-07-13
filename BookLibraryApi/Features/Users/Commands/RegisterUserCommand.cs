using BookLibraryApi.Models.Dtos;
using MediatR;

namespace BookLibraryApi.Features.Users.Commands
{
    public record RegisterUserCommand(RegisterUserDto dto) : IRequest<IResult>;
}
