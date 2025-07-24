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
        [Theory]
        [InlineData("abc",1,true)]
        [InlineData("ghost",999, false)]
        public async Task CreateBook(string Username,int? id, bool ShouldSucced)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var hasher = new PasswordHasher<User>();
            var mapper = new AutoMapper.Mapper(new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }));
            var book = new BookCreateDto {
                Title = "Test Book",
                Author = "Tester",
                Year = DateTime.UtcNow,
                Description = "Dummy"
            };
            if (ShouldSucced)
            {
                var user = TestFactory.CreateTestUser(Username, "abc", out hasher,id);
                await context.AddAsync(user);
                await context.SaveChangesAsync();
            }
            var claims = TestFactory.CreateClaimsPrincipal(Username,id);
            var command = new CreateBookCommand(book, claims);
            var handler = new CreateBookCommandHandler(context, mapper);

            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
            {
                Assert.IsType<Created>(result);
            }
            else
                Assert.IsType<UnauthorizedHttpResult>(result);
        }
    }
}
