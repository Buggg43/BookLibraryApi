using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryApi.Tests.Features.Users.Commands
{
    public class DeleteBookCommandHandlerTest
    {
        public static IEnumerable<object[]> TestDeleteBookCases =>
            new List<object[]>
            {
            new object[] { 2, true },
            new object[] { 3, false },
            new object[] { 4, false }
            };

        [Theory]
        [MemberData(nameof(TestDeleteBookCases))]
        public async Task DeleteBookTest(int id, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var claims = TestFactory.CreateClaimsPrincipal("Test", id);

            var user = TestFactory.CreateTestUser("abc", "abc", out _, id);
            var book = TestFactory.CreateTestBook(id, user.Id, "testTitle");

            if (id == 3)
                id = 5;
            else if (id == 4)
                book.UserId = 1;

            await TestFactory.AddUsersAsync(context, user);
            await TestFactory.AddBooksAsync(context, book);

            var command = new DeleteBookCommand(id, claims);
            var handler = new DeleteBookCommandHandler(context);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
            {
                Assert.IsType<NoContent>(result);
                Assert.False(await context.Books.AnyAsync(b => b.Id == id));
            }
            else if (user.Id != book.UserId)
                Assert.IsType<ForbidHttpResult>(result);
            else
                Assert.IsType<NotFound>(result);
        }
    }

}
