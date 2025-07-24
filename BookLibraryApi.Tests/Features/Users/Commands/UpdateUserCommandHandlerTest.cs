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
    public class UpdateUserCommandHandlerTest
    {
        public static IEnumerable<object[]> UpdateUserTestCases =>
            new List<object[]>
            {
                new object[] { new UpdateUserDto { UserName = "NewUsername" }, 1, true }, // correct
                new object[] { new UpdateUserDto { UserName = "OldUsername" }, 2, false }, // dto with old username
                new object[] { new UpdateUserDto { UserName = "ghost" }, 3, false }  // no user
            };
        
        [Theory]
        [MemberData(nameof(UpdateUserTestCases))]
        public async Task UpdateUserTest(UpdateUserDto dto,int id, bool ShouldSucced)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var mapper = TestFactory.CreateMapper();
            var hasher = new PasswordHasher<User>();
            var user = new User();

            if(ShouldSucced)
            {
                user = TestFactory.CreateTestUser("OldUsername", "abc",out hasher, id);
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }
            else if(id == 2)
            {
                user = TestFactory.CreateTestUser("OldUsername", "abc", out hasher, id);
                await context.Users.AddAsync(user);
                var otherUser = TestFactory.CreateTestUser("OldUsername", "abc", out hasher, id: 999);
                await context.Users.AddAsync(otherUser);
                await context.SaveChangesAsync();
            }

            var claim = TestFactory.CreateClaimsPrincipal(user.Username, id);


            var command = new UpdateUserCommand(claim,dto);
            var handler = new UpdateUserCommandHandler(context, mapper, hasher);

            var result = await handler.Handle(command, CancellationToken.None);

            if(ShouldSucced)
            {
                Assert.IsType<NoContent>(result);
            }
            else
            {
                if(id == 2)
                {  
                    Assert.IsType<Conflict<string>>(result);
                }
                else
                    Assert.IsType<NotFound>(result);
            }
        }
    }
}
