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
    public class UpdateBookCommandHandlerTest
    {
        public static IEnumerable<object[]> UpdateBookTestCases =>
        new List<object[]>
        {
                new object[] { new BookUpdateDto { Title ="NewTitle", Author ="NewTitle", Year = DateTime.UtcNow, isFavorite = false, isRead = false , Description = null }, true, 1, 1}, // OK
                new object[] { new BookUpdateDto { Title ="Title", Author ="Author", Year = DateTime.UtcNow, isFavorite = false, isRead = false , Description = null },false, 888,887},  // no book
                new object[] { new BookUpdateDto { Title = "OldTitle", Author = "OldAuthor", Year = DateTime.UtcNow, isFavorite = false, isRead = false, Description = null },false, 999,998 } // no 
        };

        [Theory]
        [MemberData(nameof(UpdateBookTestCases))]
        public async Task UpdateBookTest(BookUpdateDto dto, bool ShouldSucced, int bookId, int userId)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var mapper = TestFactory.CreateMapper();
            var hasher = new PasswordHasher<User>();
            var user = TestFactory.CreateTestUser("abc", "abc", out hasher, 1);

            if (ShouldSucced)
            {

                var book = new Book
                {
                    Id = bookId,
                    Title = "Some title",
                    Author = "Some author",
                    Description = "test",
                    Year = DateTime.UtcNow,
                    UserId = userId
                };
                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();
            }
            else if(bookId == 999)
            {
                var book = new Book
                {
                    Id = bookId,
                    Title = dto.Title,
                    Author = dto.Author,
                    Description = dto.Description,
                    Year = dto.Year,
                    UserId = userId
                };
                await context.Books.AddAsync(book);
                await context.SaveChangesAsync();
            }

            var claim = TestFactory.CreateClaimsPrincipal(user.Username, user.Id);

            var command = new UpdateBookCommand(bookId,dto, claim);
            var handler = new UpdateBookCommandHandler(context, mapper);

            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
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
