using BookLibraryApi.Models.Dtos;
using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public record LogoutCommand(LogOutRequestDto dto, ClaimsPrincipal user): IRequest<IResult>;
}
