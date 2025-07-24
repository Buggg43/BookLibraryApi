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
        [Theory]
        [InlineData("abc","abc",1,true)]
        [InlineData("abc", "abc",2, false)]
        [InlineData("ghost", "abc",3, false)]
        public async Task LogoutTest(string Username,string Password,int? id, bool ShouldSucced)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var claims = TestFactory.CreateClaimsPrincipal(Username,id);
            var hasher = new PasswordHasher<User>();
            var jwt = TestFactory.CreateJwtServiceMock();
            var logoutDto = new LogOutRequestDto { };

            if (Username != "ghost")
            {
                var user = TestFactory.CreateTestUser(Username, Password, out hasher,id);
                await context.Users.AddAsync(user);
                var token = jwt.Object.GenerateRefreshToken(user);
                if(ShouldSucced)
                {
                    var rToken = TestFactory.CreateRefreshToken(user,jwt.Object);
                    await context.RefreshTokens.AddAsync(rToken);
                }

                await context.SaveChangesAsync();

                logoutDto.refreshToken = token.Token;
            }
            var command = new LogoutCommand(logoutDto,claims);
            var handler = new LogoutCommandHandler(context);

            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
            {
                Assert.IsType<NoContent>(result);
                var tokens = context.RefreshTokens.Where(r => r.UserId == id).ToList();
                Assert.All(tokens, t => Assert.True(t.isRevoked));
            }
            else if (Username == "ghost")
                Assert.IsType<NotFound>(result);
            else
                Assert.IsType<ForbidHttpResult>(result);
                

        }
    }
}
