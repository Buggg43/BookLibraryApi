using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models;
using BookLibraryApi.Models.Dtos;
using BookLibraryApi.Tests.Common;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
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
            new object[] { new UpdateUserRoleDto { Role = "Premium", UserId = 5 }, 1, "Admin", true },
            new object[] { new UpdateUserRoleDto { Role = "Admin", UserId = 3 }, 2, "Admin", false },
            new object[] { new UpdateUserRoleDto { Role = null, UserId = 6 }, 3, "Admin", false },
            new object[] { new UpdateUserRoleDto { Role = "User", UserId = 1 }, 1, "Admin", false },
            new object[] { new UpdateUserRoleDto { Role = "Admin", UserId = 7 }, 4, "User", false }
            };

        [Theory]
        [MemberData(nameof(UpdateRoleTestCases))]
        public async Task UpdateUserRoleTest(UpdateUserRoleDto dto, int id, string role, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var claim = TestFactory.CreateClaimsPrincipal("Test", id, role);

            var adminUser = TestFactory.CreateTestUser("Test", "abc", out _, id);
            adminUser.Role = role;

            var targetUser = TestFactory.CreateTestUser("Target", "abc", out _, dto.UserId);
            targetUser.Role = dto.UserId == 3 ? "Admin" : "User";

            if (dto.UserId != 1)
                await TestFactory.AddUsersAsync(context, targetUser);

            await TestFactory.AddUsersAsync(context, adminUser);

            var command = new UpdateUserRoleCommand(dto, claim);
            var handler = new UpdateUserRoleCommandHandler(context);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
            {
                Assert.IsType<NoContent>(result);
                var updatedUser = await context.Users.FindAsync(dto.UserId);
                Assert.Equal(dto.Role, updatedUser.Role);
            }
            else if (role != "Admin" || dto.UserId == id)
            {
                Assert.IsType<ForbidHttpResult>(result);
            }
            else if (dto.Role == role || dto.Role == null)
            {
                Assert.IsType<BadRequest>(result);
            }
            else
            {
                Assert.IsType<NotFound>(result);
            }
        }
    }

}
