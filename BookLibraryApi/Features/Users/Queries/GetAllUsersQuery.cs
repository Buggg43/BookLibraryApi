using MediatR;

namespace BookLibraryApi.Features.Users.Queries
{
    public record GetAllUsersQuery() : IRequest<IResult>;
}
