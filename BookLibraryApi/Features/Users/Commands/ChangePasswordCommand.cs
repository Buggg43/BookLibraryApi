﻿using BookLibraryApi.Models.Dtos;
using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Commands
{
    public record ChangePasswordCommand(ClaimsPrincipal User, ChangePasswordDto dto ) : IRequest<IResult>;
}
