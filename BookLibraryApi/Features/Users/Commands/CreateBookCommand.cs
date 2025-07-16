using BookLibraryApi.Models.Dtos;
using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public record CreateBookCommand(BookCreateDto dto, ClaimsPrincipal User) : IRequest<IResult>;
}
