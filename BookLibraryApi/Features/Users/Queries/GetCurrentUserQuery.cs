using BookLibraryApi.Models.Dtos;
using System.Security.Claims;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Services;

namespace BookLibraryApi.Features.Users.Queries
{
    public class GetCurrentUserQuery
    {
        public UserReadDto Execute(ClaimsPrincipal user)
        {
            var userId =int.Parse( user.FindFirst(ClaimTypes.NameIdentifier).Value);
            var username = user.FindFirst(ClaimTypes.Name).Value;
            var role = user.FindFirst(ClaimTypes.Role).Value ?? "User";

            return new UserReadDto
            {
                Id = userId,
                Username = username,
                Role = role,
            };
        }
    }
}
