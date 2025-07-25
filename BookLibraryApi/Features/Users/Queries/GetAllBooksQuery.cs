using BookLibraryApi.Models.Dtos;
using MediatR;

namespace BookLibraryApi.Features.Users.Queries
{
    public record GetAllBooksQuery(GetUserBooksDto dto) : IRequest<IResult>;
}
