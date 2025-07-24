using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models;
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
    public class LogoutAllSessionsCommandHandlerTest
    {
        [Theory]
        [InlineData("abc", 1, true)]
        [InlineData("ghost", 999, false)]
        public async Task LogoutAllTest(string Username, int? userId, bool ShouldSuceed)
        {
            var claims = TestFactory.CreateClaimsPrincipal(Username, userId);
            var hasher = new PasswordHasher<User>();
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            if (Username == "abc")
            {
                var user = TestFactory.CreateTestUser(Username, "abc", out hasher, userId);
                await context.Users.AddAsync(user);

                var userr = await context.Users.FirstOrDefaultAsync(b => b.Id == userId);
                context.RefreshTokens.AddRange(new[]
                {
                    new RefreshToken { UserId = userId.Value, Expires = DateTime.UtcNow.AddDays(1), isRevoked = false, Token = "refresh_token_test1", User = userr},
                    new RefreshToken { UserId = userId.Value, Expires = DateTime.UtcNow.AddDays(-1), isRevoked = false, Token = "refresh_token_test2", User = userr},
                    new RefreshToken { UserId = userId.Value, Expires = DateTime.UtcNow.AddDays(1), isRevoked = true, Token = "refresh_token_test3", User = userr}
                });
                await context.SaveChangesAsync();
            }


            var command = new LogoutAllCommand(claims);
            var handler = new LogoutAllCommandHandler(context); 

            var result =await handler.Handle(command, CancellationToken.None);
            var tokens = context.RefreshTokens.Where(b => b.UserId == userId).ToList();
            if (ShouldSuceed)
            {
                Assert.IsType<NoContent>(result);

                var now = DateTime.UtcNow;

                Assert.All(tokens.Where(t => t.Expires > now), token =>
                {
                    Assert.True(token.isRevoked);
                });
            }
            else
            {
                Assert.IsType<UnauthorizedHttpResult>(result); 
            }
        }
    }
}
