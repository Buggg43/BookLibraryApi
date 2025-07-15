using BookLibraryApi.Models.Dtos;
using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Queries
{
    public record GetUserBooksQuery(GetUserBooksDto dto, ClaimsPrincipal user) : IRequest<IResult>;
    
}
