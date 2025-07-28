using AutoMapper;
using BookLibraryApi.Features.Users.Queries;
using BookLibraryApi.Mapper;
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

namespace BookLibraryApi.Tests.Features.Users.Queries
{
    public class GetCurrentUserQueryHandlerTest
    {
        public static IEnumerable<object[]> GetCurrentUserTestCases =>
        new List<object[]>
        {
            new object[] { "existing_user", true },
            new object[] { "ghost", false }
        };
        [Theory]
        [MemberData(nameof(GetCurrentUserTestCases))]
        public async Task GetCurrenUserQueryHandlerTest(string Username, bool ShouldSucced)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var loggerFactory = LoggerFactory.Create(lb => lb.AddDebug());
            var mapper = TestFactory.CreateMapper(loggerFactory);

            if (ShouldSucced)
            {
                var user = TestFactory.CreateTestUser(Username, "abc", out _);
                await TestFactory.AddUsersAsync(context, user);
            }
            var claims = TestFactory.CreateClaimsPrincipal(Username);

            var handler = new GetCurrentUserQueryHandler(mapper,context);
            var command = new GetCurrentUserQuery(claims);

            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
            {
                var okResult = Assert.IsType<Ok<UserReadDto>>(result);
                Assert.Equal("existing_user", okResult.Value.Username);
            }
            else
                Assert.IsType<BadRequest>(result);
            
        }
    }
}
