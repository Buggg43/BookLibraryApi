using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace BookLibraryApi.Tests.Features.Users.Commands
{
    public class LogoutAllSessionsCommandHandlerTest
    {
        public static IEnumerable<object[]> LogoutAllSessionsCases =>
            new List<object[]>
            {
                new object[] { "abc", 1, true },
                new object[] { "ghost", 2, false }
            };

        [Theory]
        [MemberData(nameof(LogoutAllSessionsCases))]
        public async Task LogoutAllSessionsTest(string username, int id, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var user = TestFactory.CreateTestUser(username, "abc", out _, id);

            if (shouldSucceed)
            {
                user.RefreshTokens = new List<RefreshToken>
                {
                    new RefreshToken { Token = "token1", Expires = DateTime.UtcNow.AddDays(1) },
                    new RefreshToken { Token = "token2", Expires = DateTime.UtcNow.AddDays(1) }
                };
                await TestFactory.AddUsersAsync(context, user);
            }

            var claims = TestFactory.CreateClaimsPrincipal(username, id);
            var command = new LogoutAllCommand(claims);
            var handler = new LogoutAllCommandHandler(context);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
            {
                Assert.IsType<NoContent>(result);
                var refreshedUser = await context.Users
                    .Include(u => u.RefreshTokens)
                    .FirstAsync(u => u.Id == id);

                Assert.All(refreshedUser.RefreshTokens, t => Assert.True(t.isRevoked));
            }
            else
            {
                Assert.IsType<UnauthorizedHttpResult>(result);
            }
        }
    }
}
