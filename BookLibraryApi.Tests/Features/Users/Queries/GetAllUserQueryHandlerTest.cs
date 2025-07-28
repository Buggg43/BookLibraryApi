using BookLibraryApi.Tests.Common;
using BookLibraryApi.Features.Users.Queries;
using Microsoft.AspNetCore.Http.HttpResults;
using BookLibraryApi.Models.Dtos;
using Microsoft.Extensions.Logging;

namespace BookLibraryApi.Tests.Features.Users.Queries
{
    public class GetAllUserQueryHandlerTest
    {
        public static IEnumerable<object[]> GetAllUsersTestCases =>
            new List<object[]>
            {
                new object[] { "abc", 1, "Admin", true },
                new object[] { "abc", 2, "User", false }
            };

        [Theory]
        [MemberData(nameof(GetAllUsersTestCases))]
        public async Task GetAllUsers(string username, int? id, string role, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var loggerFactory = LoggerFactory.Create(lb => lb.AddDebug());
            var mapper = TestFactory.CreateMapper(loggerFactory);

            var claim = TestFactory.CreateClaimsPrincipal(username, id, role);

            var query = new GetAllUsersQuery(claim);
            var handler = new GetAllUsersQueryHandler(context, mapper);

            var result = await handler.Handle(query, CancellationToken.None);

            if (shouldSucceed)
                Assert.IsType<Ok<List<UserReadDto>>>(result);
            else
                Assert.IsType<ForbidHttpResult>(result);
        }
    }
}
