using BookLibraryApi.Models.Dtos;
using MediatR;

namespace BookLibraryApi.Features.Users.Commands
{
    public record RefreshTokenCommand(RefreshTokenRequestDto dto):IRequest<IResult>;

}
