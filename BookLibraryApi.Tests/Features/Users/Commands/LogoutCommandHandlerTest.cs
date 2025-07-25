using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryApi.Tests.Features.Users.Commands
{
    public class LogoutCommandHandlerTest
    {
        public static IEnumerable<object[]> LogoutCases =>
            new List<object[]>
            {
            new object[] { "abc", "abc", 1, true },
            new object[] { "abc", "abc", 2, false },
            new object[] { "ghost", "abc", 3, false }
            };

        [Theory]
        [MemberData(nameof(LogoutCases))]
        public async Task LogoutTest(string username, string password, int? id, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var claims = TestFactory.CreateClaimsPrincipal(username, id);
            var jwt = TestFactory.CreateJwtServiceMock();

            var logoutDto = new LogOutRequestDto();

            if (username != "ghost")
            {
                var user = TestFactory.CreateTestUser(username, password, out _, id);
                await TestFactory.AddUsersAsync(context, user);

                var token = jwt.Object.GenerateRefreshToken(user);
                logoutDto.refreshToken = token.Token;

                if (shouldSucceed)
                {
                    var rToken = TestFactory.CreateRefreshToken(user, jwt.Object);
                    await context.RefreshTokens.AddAsync(rToken);
                    await context.SaveChangesAsync();
                }
            }

            var command = new LogoutCommand(logoutDto, claims);
            var handler = new LogoutCommandHandler(context);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
            {
                Assert.IsType<NoContent>(result);
                var tokens = context.RefreshTokens.Where(r => r.UserId == id).ToList();
                Assert.All(tokens, t => Assert.True(t.isRevoked));
            }
            else if (username == "ghost")
            {
                Assert.IsType<NotFound>(result);
            }
            else
            {
                Assert.IsType<ForbidHttpResult>(result);
            }
        }
    }

}
