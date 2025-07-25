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
        public static IEnumerable<object[]> RefreshTokenCases =>
            new List<object[]>
            {
        new object[] { "valid_token", 1, true },
        new object[] { "expired_token", 999, false },
        new object[] { "revoked_token", 998, false },
        new object[] { "ghost_token", 997, false }
            };

        [Theory]
        [MemberData(nameof(RefreshTokenCases))]
        public async Task RefreshTokenTest(string token, int? id, bool shouldSucceed)
        {
            var dto = new RefreshTokenRequestDto { token = token };
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var jwt = TestFactory.CreateJwtServiceMock();

            var user = TestFactory.CreateTestUser("user", "abc", out _, id);
            await TestFactory.AddUsersAsync(context, user);

            var userId = user.Id;

            context.RefreshTokens.AddRange(new[]
            {
                new RefreshToken { UserId = userId, Expires = DateTime.UtcNow.AddDays(1), isRevoked = false, Token = "valid_token" },
                new RefreshToken { UserId = userId, Expires = DateTime.UtcNow.AddDays(-1), isRevoked = false, Token = "expired_token" },
                new RefreshToken { UserId = userId, Expires = DateTime.UtcNow.AddDays(1), isRevoked = true, Token = "revoked_token" },
                new RefreshToken { UserId = userId, Expires = DateTime.UtcNow.AddDays(-1), isRevoked = true, Token = "ghost_token" }
            });

            await context.SaveChangesAsync();

            var command = new RefreshTokenCommand(dto);
            var handler = new RefreshTokenCommandHandler(context, jwt.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
                Assert.IsType<Ok<TokenPairDto>>(result);
            else
                Assert.IsType<UnauthorizedHttpResult>(result);
        }
    }

 }
