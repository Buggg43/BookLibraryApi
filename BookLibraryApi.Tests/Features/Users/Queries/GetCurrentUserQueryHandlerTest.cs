using AutoMapper;
using BookLibraryApi.Features.Users.Queries;
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

namespace BookLibraryApi.Tests.Features.Users.Queries
{
    public class GetCurrentUserQueryHandlerTest
    {
        [Theory]
        [InlineData("existing_user", true)]
        [InlineData("ghost", false)]
        public async Task GetCurrenUserQueryHandlerTest(string Username, bool ShouldSucced)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var hasher = new PasswordHasher<User>();
            if (ShouldSucced)
            {
                var user = TestFactory.CreateTestUser(Username, "abc", out hasher);
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
            var claims = TestFactory.CreateClaimsPrincipal(Username);
            var mapper = new AutoMapper.Mapper(new MapperConfiguration(cfg =>
                cfg.AddProfile<MappingProfile>()
            ));


            var handler = new GetCurrentUserQueryHandler(mapper,context);
            var command = new GetCurrentUserQuery(claims);

            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
            {
                var okResult = result as Ok<UserReadDto>;
                Assert.NotNull(okResult);
                Assert.NotNull(okResult.Value);
                Assert.Equal("existing_user",okResult.Value.Username);
            }
            else
                Assert.IsType<BadRequest>(result);
            
        }
    }
}
