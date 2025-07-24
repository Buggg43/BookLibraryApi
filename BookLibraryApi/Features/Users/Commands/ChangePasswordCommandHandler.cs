using BookLibraryApi.Data;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class ChangePasswordCommandHandler : IRequestHandler<ChangePasswordCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly PasswordHasher<User> _hasher;

        public ChangePasswordCommandHandler(LibraryDbContext context, PasswordHasher<User> hasher)
        {
            _context = context;
            _hasher = hasher;
        }
        public async Task<IResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(request.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Id == userId);
            if (user == null) 
                return Results.NotFound();

            var hash = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.dto.OldPassword);
            if (hash == PasswordVerificationResult.Failed)
                return Results.Unauthorized();

            var isSamePassword = _hasher.VerifyHashedPassword(user, user.PasswordHash, request.dto.NewPassword);
            if (isSamePassword == PasswordVerificationResult.Success)
                return Results.BadRequest("New password must be different from the old password.");

            user.PasswordHash = _hasher.HashPassword(user,request.dto.NewPassword);
            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
