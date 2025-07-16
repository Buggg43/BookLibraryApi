using BookLibraryApi.Models.Dtos;
using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public record UpdateBookCommand(int BookId,BookUpdateDto dto, ClaimsPrincipal User) : IRequest<IResult>;
}
