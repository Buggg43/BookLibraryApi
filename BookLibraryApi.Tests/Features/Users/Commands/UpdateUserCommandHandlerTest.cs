using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryApi.Tests.Features.Users.Commands
{
    public class UpdateUserCommandHandlerTest
    {
        public static IEnumerable<object[]> UpdateUserTestCases =>
            new List<object[]>
            {
            new object[] { new UpdateUserDto { UserName = "NewUsername" }, 1, true },
            new object[] { new UpdateUserDto { UserName = "OldUsername" }, 2, false },
            new object[] { new UpdateUserDto { UserName = "ghost" }, 3, false }
            };

        [Theory]
        [MemberData(nameof(UpdateUserTestCases))]
        public async Task UpdateUserTest(UpdateUserDto dto, int id, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var loggerFactory = LoggerFactory.Create(lb => lb.AddDebug());
            var mapper = TestFactory.CreateMapper(loggerFactory);
            var hasher = new PasswordHasher<User>();

            User user = null;

            if (shouldSucceed || id == 2)
            {
                user = TestFactory.CreateTestUser("OldUsername", "abc", out hasher, id);
                await TestFactory.AddUsersAsync(context, user);

                if (id == 2)
                {
                    var otherUser = TestFactory.CreateTestUser("OldUsername", "abc", out hasher, 999);
                    await TestFactory.AddUsersAsync(context, otherUser);
                }
            }

            var claim = TestFactory.CreateClaimsPrincipal(user?.Username ?? dto.UserName, id);
            var command = new UpdateUserCommand(claim, dto);
            var handler = new UpdateUserCommandHandler(context, mapper, hasher);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
            {
                Assert.IsType<NoContent>(result);
            }
            else if (id == 2)
            {
                Assert.IsType<Conflict<string>>(result);
            }
            else
            {
                Assert.IsType<NotFound>(result);
            }
        }
    }

}
