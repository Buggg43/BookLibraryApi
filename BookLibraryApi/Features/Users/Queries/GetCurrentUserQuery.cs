﻿using MediatR;
using System.Security.Claims;

namespace BookLibraryApi.Features.Users.Queries
{
    public record GetCurrentUserQuery(ClaimsPrincipal user) : IRequest<IResult>;
}
