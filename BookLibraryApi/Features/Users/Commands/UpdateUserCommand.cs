using AutoMapper;
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
    public record UpdateUserCommand(ClaimsPrincipal User, UpdateUserDto Dto) : IRequest<IResult>;
}


/*
 *      CQRS bez MediatR
 *      
 *      private readonly LibraryDbContext _context;
        private readonly PasswordHasher<User> _hasher;
        private readonly IMapper _mapper;
        public UpdateUserCommand(LibraryDbContext context, PasswordHasher<User> hasher, IMapper mapper) 
        {
            _context = context;
            _hasher = hasher;
            _mapper = mapper;
        }
        public async Task<IActionResult> ExecuteAsync(ClaimsPrincipal authUser, UpdateUserDto dto)
        {
            var userId = int.Parse(authUser.FindFirst(ClaimTypes.NameIdentifier).Value);
            var user = await _context.Users.FirstOrDefaultAsync(b => b.Id == userId);
            if (user == null) 
                return new NotFoundResult();

            var usernameTaken = await _context.Users.AnyAsync(u => u.Username == dto.UserName && u.Id != userId);
            if (usernameTaken)
                return new ConflictResult();

            _mapper.Map(dto, user);

            await _context.SaveChangesAsync();

            return new NoContentResult();

        }
*/