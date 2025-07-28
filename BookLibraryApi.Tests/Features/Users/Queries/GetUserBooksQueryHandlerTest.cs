using BookLibraryApi.Features.Users.Queries;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryApi.Tests.Features.Users.Queries
{
    public class GetUserBooksQueryHandlerTest
    {
        public static IEnumerable<object[]> UserBooksTestCases =>
            new List<object[]>
            {
                new object[] {new GetUserBooksDto{ Page = 1, PageSize = 2 },2 , 3, 2 },
                new object[] {new GetUserBooksDto { Page = 1, PageSize = 10, isRead = true}, 1, 1, 1 },
                new object[] {new GetUserBooksDto { Page = 1, PageSize = 10, Title = "testTitleA"}, 2, 2, 1 },
                new object[] {new GetUserBooksDto { Page = 2, PageSize = 2}, 1, 3, 2 }
            };

        [Theory]
        [MemberData(nameof(UserBooksTestCases))]
        public async Task GetUserBooks(GetUserBooksDto dto, int booksReturned, int expectedCount, int expectedTotalPages)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var loggerFactory = LoggerFactory.Create(lb => lb.AddDebug());
            var mapper = TestFactory.CreateMapper(loggerFactory);

            var user = TestFactory.CreateTestUser("testUser", "abc", out _, 1);
            await TestFactory.AddUsersAsync(context, user);

            var book1 = TestFactory.CreateTestBook(1, user.Id, "testTitleA", true);
            var book2 = TestFactory.CreateTestBook(2, user.Id, "testTitleA");
            var book3 = TestFactory.CreateTestBook(3, user.Id, "testTitleB");

            await TestFactory.AddBooksAsync(context, book1, book2, book3);

            var claim = TestFactory.CreateClaimsPrincipal("Test", user.Id);

            var command = new GetUserBooksQuery(dto, claim);
            var handler = new GetUserBooksQueryHandler(context, mapper);

            var result = await handler.Handle(command, CancellationToken.None);

            var okResult = Assert.IsType<Ok<GetUserBooksResponseDto>>(result);
            Assert.Equal(expectedCount, okResult.Value.TotalItems);
            Assert.Equal(expectedTotalPages, okResult.Value.TotalPages);
            Assert.Equal(booksReturned, okResult.Value.Data.Count);


        }
    }
}
