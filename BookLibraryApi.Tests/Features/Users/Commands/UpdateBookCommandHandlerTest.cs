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
    public class UpdateBookCommandHandlerTest
    {
        public static IEnumerable<object[]> UpdateBookTestCases =>
            new List<object[]>
            {
            new object[] { new BookUpdateDto { Title = "NewTitle", Author = "NewAuthor", Year = DateTime.UtcNow, isFavorite = false, isRead = false, Description = null }, true, 1, 1 },
            new object[] { new BookUpdateDto { Title = "Title", Author = "Author", Year = DateTime.UtcNow, isFavorite = false, isRead = false, Description = null }, false, 888, 887 },
            new object[] { new BookUpdateDto { Title = "OldTitle", Author = "OldAuthor", Year = DateTime.UtcNow, isFavorite = false, isRead = false, Description = null }, false, 999, 998 }
            };

        [Theory]
        [MemberData(nameof(UpdateBookTestCases))]
        public async Task UpdateBookTest(BookUpdateDto dto, bool shouldSucceed, int bookId, int userId)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var loggerFactory = LoggerFactory.Create(lb => lb.AddDebug());
            var mapper = TestFactory.CreateMapper(loggerFactory);
            var user = TestFactory.CreateTestUser("abc", "abc", out _, 1);

            if (shouldSucceed)
            {
                var book = TestFactory.CreateTestBook(bookId, userId, "Some title");
                await TestFactory.AddBooksAsync(context, book);
            }
            else if (bookId == 999)
            {
                var book = TestFactory.CreateTestBook(bookId, userId, dto.Title);
                book.Author = dto.Author;
                await TestFactory.AddBooksAsync(context, book);
            }

            var claim = TestFactory.CreateClaimsPrincipal(user.Username, user.Id);
            var command = new UpdateBookCommand(bookId, dto, claim);
            var handler = new UpdateBookCommandHandler(context, mapper);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
            {
                Assert.IsType<NoContent>(result);

                var updatedBook = await context.Books.FindAsync(bookId);
                Assert.Equal(dto.Title, updatedBook.Title);
                Assert.Equal(dto.Author, updatedBook.Author);
            }
            else if (bookId == 888)
                Assert.IsType<NotFound>(result);
            else
                Assert.IsType<ForbidHttpResult>(result);
        }
    }



}
