using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryApi.Tests.Features.Users.Commands
{
    public class RemoveUserCommandHandlerTest
    {
        public static IEnumerable<object[]> RemoveUserCases =>
            new List<object[]>
            {
            new object[] { 1, "User", true },
            new object[] { 2, "Admin", false },
            new object[] { 3, "User", false }
            };

        [Theory]
        [MemberData(nameof(RemoveUserCases))]
        public async Task RemoveUserTest(int id, string role, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());

            var user = TestFactory.CreateTestUser("Test", "abc", out _, id);
            user.Role = role;

            var book = TestFactory.CreateTestBook(id, id, "testTitle");

            if (id != 3)
                await TestFactory.AddUsersAsync(context, user);

            await TestFactory.AddBooksAsync(context, book);

            var command = new RemoveUserCommand(id);
            var handler = new RemoveUserCommandHandler(context);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
            {
                Assert.IsType<NoContent>(result);
                Assert.False(context.Users.Any(u => u.Id == id));
                Assert.False(context.Books.Any(b => b.UserId == id));
            }
            else if (role == "Admin")
                Assert.IsType<ForbidHttpResult>(result);
            else
                Assert.IsType<NotFound>(result);
        }
    }

}
