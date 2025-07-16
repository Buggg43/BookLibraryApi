using BookLibraryApi.Models.Dtos;
using MediatR;

namespace BookLibraryApi.Features.Users.Commands
{
    public record UpdateUserRoleCommand(UpdateUserRoleDto dto) : IRequest<IResult>;

}
