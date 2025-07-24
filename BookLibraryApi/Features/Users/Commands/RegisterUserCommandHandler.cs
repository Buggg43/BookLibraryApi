using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using BookLibraryApi.Services;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApi.Features.Users.Commands
{
    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;
        private readonly PasswordHasher<User> _hasher;
        
        public RegisterUserCommandHandler(LibraryDbContext context, IMapper mapper, PasswordHasher<User> hasher) 
        {
            _context = context;
            _mapper = mapper;
            _hasher = hasher;
            
        }
        public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.dto;
            if (dto.Username == null || dto.Password == null)
                return Results.BadRequest();

            var exist = await _context.Users.AnyAsync(b => b.Username == dto.Username);
            if(exist)
                return Results.Conflict("Username taken");

            var user = new User 
            {
                Username = dto.Username,
                Role = "User"
            };
            user.PasswordHash = _hasher.HashPassword(user, dto.Password);

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return Results.Created();
        }
    }
}
