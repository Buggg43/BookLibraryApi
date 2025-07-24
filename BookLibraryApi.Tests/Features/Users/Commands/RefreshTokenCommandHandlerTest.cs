using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryApi.Tests.Features.Users.Commands
{
    public class RefreshTokenCommandHandlerTest
    {
        [Theory]
        [InlineData("valid_token",1,true)]
        [InlineData("expired_token", 999,false)]
        [InlineData("revoked_token", 998, false)]
        [InlineData("ghost_token", 997, false)]
        public async Task RefreshTokeTest(string Username, int? id,bool ShouldSucced)
        {
            var dto = new RefreshTokenRequestDto { token = Username.Trim() };
            var hasher = new PasswordHasher<User>();
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var jwt = TestFactory.CreateJwtServiceMock();
            var user = TestFactory.CreateTestUser(Username, "abc", out hasher,id );

            await context.Users.AddAsync(user);

            var userId = user.Id;

            context.RefreshTokens.AddRange(new[]
            {
                    new RefreshToken { UserId = userId, Expires = DateTime.UtcNow.AddDays(1), isRevoked = false, Token = "valid_token" },
                    new RefreshToken { UserId = userId, Expires = DateTime.UtcNow.AddDays(-1), isRevoked = false, Token = "expired_token" },
                    new RefreshToken { UserId = userId, Expires = DateTime.UtcNow.AddDays(1), isRevoked = true, Token = "revoked_token"},
                    new RefreshToken { UserId = userId, Expires = DateTime.UtcNow.AddDays(-1), isRevoked = true, Token = "ghost_token"}
            });
            await context.SaveChangesAsync();

            var command = new RefreshTokenCommand(dto);
            var handler = new RefreshTokenCommandHandler(context, jwt.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
            {
                Assert.IsType<Ok<TokenPairDto>>(result);
            }
            else
                Assert.IsType<UnauthorizedHttpResult>(result);
        }
    }
}
