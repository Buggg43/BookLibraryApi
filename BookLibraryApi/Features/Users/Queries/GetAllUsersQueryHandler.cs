﻿using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Models.Dtos;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Queries
{
    public class GetAllUsersQueryHandler : IRequestHandler<GetAllUsersQuery, IResult>
    {
        private readonly LibraryDbContext _context;
        private readonly IMapper _mapper;
        public GetAllUsersQueryHandler(LibraryDbContext context, IMapper mapper)
        {
            _mapper = mapper;
            _context = context;
        }
        public async Task<IResult> Handle(GetAllUsersQuery request, CancellationToken cancellationToken)
        {
            var role = request.user.FindFirst(ClaimTypes.Role)?.Value.ToString();
            if (role != "Admin")
                return Results.Forbid();

            var allUsers = await _context.Users.ToListAsync();
            var result = _mapper.Map<List<UserReadDto>>(allUsers);

            return Results.Ok(result);
        }

    }
}
