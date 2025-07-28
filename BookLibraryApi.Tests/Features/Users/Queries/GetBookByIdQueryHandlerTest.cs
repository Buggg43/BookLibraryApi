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
    public class GetBookByIdQueryHandlerTest
    {
        public static IEnumerable<object[]> GetBookTestCases =>
            new List<object[]>
            {
                new object[] { 2, true },  
                new object[] { 3, false }, 
                new object[] { 4, false } 
            };
        [Theory]
        [MemberData(nameof(GetBookTestCases))]
        public async Task GetBookByIdTest(int id, bool ShouldSucced)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var claim = TestFactory.CreateClaimsPrincipal("Test",id);
            var loggerFactory = LoggerFactory.Create(lb => lb.AddDebug());
            var mapper = TestFactory.CreateMapper(loggerFactory);
            var testUser = TestFactory.CreateTestUser("abc", "abc", out _, id);
            await TestFactory.AddUsersAsync(context, testUser);

            Book testBook;

            if (id == 3)
                testBook = TestFactory.CreateTestBook(id, 10, "testTitle");
            else
                testBook = TestFactory.CreateTestBook(id, testUser.Id, "testTitle");

            if (id != 4)
                await TestFactory.AddBooksAsync(context, testBook);

            var command = new GetBookByIdQuery(id,claim);
            var handler = new GetBookByIdQueryHandler(context, mapper);
            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
            {
                var okResult = Assert.IsType<Ok<BookReadDto>>(result);
                var dto = okResult.Value;
                Assert.Equal(testBook.Title, dto.Title);
                Assert.Equal(testBook.Author, dto.Author);
            }
            else if (context.Books.Any(b => b.Id == id))
                Assert.IsType<ForbidHttpResult>(result);
            else
                Assert.IsType<NotFound>(result);

        }
    }
}
