﻿using BookLibraryApi.Models.Dtos;
using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public record UpdateUserCommand(ClaimsPrincipal User, UpdateUserDto Dto) : IRequest<IResult>;
}
