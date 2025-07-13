using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

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
        }
        public async Task<IResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
        {
            var dto = request.dto;

            if (dto.Username == null || dto.Password == null)
                return Results.BadRequest();
            


            return Results.NoContent();
        }
    }
}
