using BookLibraryApi.Data;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class ChangePasswordCommand
    {
        private readonly LibraryDbContext _context;
        private readonly PasswordHasher<User> _hasher;

        public ChangePasswordCommand(LibraryDbContext context, PasswordHasher<User> hasher)
        {
            _context = context;
            _hasher = hasher;
        }
        public async Task<IActionResult> ExecuteAsync(ClaimsPrincipal authUser, ChangePasswordDto dto)
        {
            var userId = int.Parse(authUser.FindFirst(ClaimTypes.NameIdentifier).Value);

            if (userId == 0)
                return new NotFoundResult();

            var user = await _context.Users.FirstOrDefaultAsync(b => b.Id == userId);
            if (user == null) return new NotFoundResult();

            var hash = _hasher.VerifyHashedPassword(user, user.PasswordHash, dto.OldPassword);
            if (hash == PasswordVerificationResult.Failed)
                return new UnauthorizedResult();

            user.PasswordHash = _hasher.HashPassword(user,dto.NewPassword);
            await _context.SaveChangesAsync();

            return new NoContentResult();
        }
    }
}
