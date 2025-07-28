using BookLibraryApi.Features.Users.Queries;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookLibraryApi.Tests.Features.Users.Queries
{
    public class GetAllBooksQueryHandlerTest
    {
        public static IEnumerable<object[]> GetAllBooksTestCases =>
            new List<object[]>
            {
                new object[] {new GetUserBooksDto{ Page = 1, PageSize = 2 },2 , 3, 2 },
                new object[] {new GetUserBooksDto { Page = 1, PageSize = 10, isRead = true}, 1, 1, 1 },
                new object[] {new GetUserBooksDto { Page = 1, PageSize = 10, Title = "testTitleA"}, 2, 2, 1 },
                new object[] {new GetUserBooksDto { Page = 2, PageSize = 2}, 1, 3, 2 }
            };
        [Theory]
        [MemberData(nameof(GetAllBooksTestCases))]
        public async Task GetAllBooksTest(GetUserBooksDto dto, int booksReturned, int expectedCount, int expectedTotalPages)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var loggerFactory = LoggerFactory.Create(lb => lb.AddDebug());
            var mapper = TestFactory.CreateMapper(loggerFactory);
            var book1 = TestFactory.CreateTestBook(1, 2, "testTitleA", true);
            var book2 = TestFactory.CreateTestBook(2, 3, "testTitleA");
            var book3 = TestFactory.CreateTestBook(3, 5, "testTitleB");

            await TestFactory.AddBooksAsync(context, book1, book2, book3);

            var command = new GetAllBooksQuery(dto);
            var handler = new GetAllBooksQueryHandler(context,mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            var okResult = Assert.IsType<Ok<GetUserBooksResponseDto>>(result);
            Assert.Equal(expectedCount, okResult.Value.TotalItems);
            Assert.Equal(expectedTotalPages, okResult.Value.TotalPages);
            Assert.Equal(booksReturned, okResult.Value.Data.Count);
        }
    }
}
