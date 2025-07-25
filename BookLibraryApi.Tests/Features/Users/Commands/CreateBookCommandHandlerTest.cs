using AutoMapper;
using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Mapper;
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
    public class CreateBookCommandHandlerTest
    {
        public static IEnumerable<object[]> CreateBookCases =>
            new List<object[]>
            {
            new object[] { "abc", 1, true },
            new object[] { "ghost", 999, false }
            };

        [Theory]
        [MemberData(nameof(CreateBookCases))]
        public async Task CreateBook(string username, int? id, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var mapper = TestFactory.CreateMapper();

            var book = new BookCreateDto
            {
                Title = "Test Book",
                Author = "Tester",
                Year = DateTime.UtcNow,
                Description = "Dummy"
            };

            if (shouldSucceed)
            {
                var user = TestFactory.CreateTestUser(username, "abc", out _, id);
                await TestFactory.AddUsersAsync(context, user);

                var claims = TestFactory.CreateClaimsPrincipal(username, id);
                var command = new CreateBookCommand(book, claims);
                var handler = new CreateBookCommandHandler(context, mapper);

                var result = await handler.Handle(command, CancellationToken.None);

                if (shouldSucceed)
                    Assert.IsType<Created>(result);
                else
                    Assert.IsType<UnauthorizedHttpResult>(result);
            }
        }
    }
}
