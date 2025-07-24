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
    public class UpdateUserRoleHandlerTest
    {
        public static IEnumerable<object[]> UpdateRoleTestCases =>
            new List<object[]>
            {
                new object[] { new UpdateUserRoleDto { Role = "Admin", UserId = 5 }, 1,"Admin", true }, // correct
                new object[] { new UpdateUserRoleDto { Role = "User", UserId = 3 }, 2, "Admin", false },  // change in to the same role
                new object[] { new UpdateUserRoleDto { Role = null, UserId = 6 }, 3, "Admin", false }, // Unknown role
                new object[] { new UpdateUserRoleDto { Role = "Admin", UserId = 1 }, 1,"Admin", false },
                new object[] { new UpdateUserRoleDto { Role = "Admin", UserId = 7 }, 4, "User", false }
            };
        [Theory]
        [MemberData(nameof(UpdateRoleTestCases))]
        public async Task UpdateUserRoleTest(UpdateUserRoleDto dto, int id,string role, bool ShouldSucced)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var claim = TestFactory.CreateClaimsPrincipal("Test", id, role);
            var hasher = new PasswordHasher<User>();

            if(ShouldSucced)
            {
                var user = TestFactory.CreateTestUser(role, "abc", out hasher);
                user.Role = role;
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();

            }

            var command = new UpdateUserRoleCommand(dto, claim);
            var handler = new UpdateUserRoleCommandHandler(context);

            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
                Assert.IsType<NoContent>(result);
            else
                if()
                    Assert.IsType<NotFound>(result);

        }

    }
}
