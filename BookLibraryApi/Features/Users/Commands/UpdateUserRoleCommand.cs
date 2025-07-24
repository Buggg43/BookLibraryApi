using BookLibraryApi.Models.Dtos;
using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public record UpdateUserRoleCommand(UpdateUserRoleDto dto, ClaimsPrincipal user) : IRequest<IResult>;

}
