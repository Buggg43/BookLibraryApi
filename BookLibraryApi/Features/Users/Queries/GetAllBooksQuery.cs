using MediatR;

namespace BookLibraryApi.Features.Users.Queries
{
    public record GetAllBooksQuery() : IRequest<IResult>;
}
