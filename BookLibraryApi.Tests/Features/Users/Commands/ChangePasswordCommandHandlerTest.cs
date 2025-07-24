using AutoMapper;
using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Mapper;
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
    public class ChangePasswordCommandHandlerTest
    {
        [Theory]
        [InlineData("abc",1, "oldpass", "newpass", true)]
        [InlineData("bca",2, "wrongpass", "newpass", false)]
        [InlineData("ghost",3, "oldpass", "newpass", false)]
        [InlineData("abd",4, "oldpass", "oldpass", false)]
        public async Task ChangePasswordCommandHandler(string Username,int?id, string OldPassword, string NewPassword, bool ShouldSucced)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var hasher = new PasswordHasher<User>();
            var jwt = TestFactory.CreateJwtServiceMock();

            if (Username != "ghost")
            {
                var user = new User { Username = Username, Role = "User" };
                user.PasswordHash = hasher.HashPassword(user, "oldpass");
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
                
            }
            var claims = TestFactory.CreateClaimsPrincipal(Username,1);
            var changePassword = new ChangePasswordDto { NewPassword = NewPassword, OldPassword = OldPassword };

            var handler = new ChangePasswordCommandHandler(context, hasher);
            var command = new ChangePasswordCommand(claims, changePassword);

            var result = await handler.Handle(command, CancellationToken.None);

            if (ShouldSucced)
            {
                Assert.IsType<NoContent>(result);

                var updateUser = await context.Users
                    .AsNoTracking()
                    .FirstOrDefaultAsync(u => u.Username == Username);

                var check = hasher.VerifyHashedPassword(updateUser, updateUser.PasswordHash, NewPassword);

                Assert.Equal(PasswordVerificationResult.Success, check);

            }
            else
            {
                if(OldPassword == NewPassword)
                    Assert.IsType<BadRequest<string>>(result);

                else if (Username == "ghost")
                    Assert.IsType<NotFound>(result);
                
                else
                    Assert.IsType<UnauthorizedHttpResult>(result);

            }

        }
    }
}