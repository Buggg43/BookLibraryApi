using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _hasher;

        public UpdateUserCommandHandler(LibraryDbContext context, IMapper mapper,PasswordHasher<User> hasher)
        {
            _context = context;
            _mapper = mapper;
            _hasher = hasher;
        }
        public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var userId = int.Parse(request.User.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Id == userId);
            if (user == null)
                return Results.NotFound(); // do poprawy
            var exist = await _context.Users.AnyAsync(b => b.Username == request.Dto.UserName && b.Id != userId);
            if (exist)
                return Results.Conflict("Username is already taken.");

            _mapper.Map(request.Dto, user);
            await _context.SaveChangesAsync();

            return Results.NoContent();
        }
    }
}
