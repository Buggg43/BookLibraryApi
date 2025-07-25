using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using BookLibraryApi.Services;
using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using BookLibraryApi.Tests.Common;

namespace BookLibraryApi.Tests.Features.Users.Commands
{
    public class LoginUserCommandHandlerTest
    {
        public static IEnumerable<object[]> LoginUserCases =>
            new List<object[]>
            {
                new object[] { "abc", "abc", true },
                new object[] { "abc", "wrong", false },
                new object[] { "ghost", "abc", false }
            };

        [Theory]
        [MemberData(nameof(LoginUserCases))]
        public async Task LoginUserTest(string username, string password, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var hasher = new PasswordHasher<User>();

            if (username != "ghost")
            {
                var user = TestFactory.CreateTestUser(username, "abc", out hasher);
                await TestFactory.AddUsersAsync(context, user);
            }

            var jwtMock = TestFactory.CreateJwtServiceMock();
            var command = new LoginUserCommand(new LoginUserDto { Username = username, Password = password });
            var handler = new LoginUserCommandHandler(context, hasher, jwtMock.Object);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
                Assert.IsType<Ok<TokenPairDto>>(result);
            else
                Assert.IsType<UnauthorizedHttpResult>(result);
        }
    }

}
