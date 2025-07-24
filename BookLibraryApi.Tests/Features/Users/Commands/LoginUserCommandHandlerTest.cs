using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using BookLibraryApi.Data;
using BookLibraryApi.Models;
using Microsoft.AspNetCore.Identity;
using Moq;
using BookLibraryApi.Services;
using BookLibraryApi.Features.Users.Commands;
using BookLibraryApi.Models.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;
using BookLibraryApi.Tests.Common;

namespace BookLibraryApi.Tests.Features.Users.Commands
{
    
    public class LoginUserCommandHandlerTest 
    {
        [Theory]
        [InlineData("123", "123", true)]
        [InlineData("123", "wrongpass", false)]
        [InlineData("nobody", "123", false)]
        public async Task Handle_LoginTests(string Username, string Password, bool shouldSucceed)
        {
            var context = TestFactory.CreateContext(Guid.NewGuid().ToString());
            var mockJwtService = TestFactory.CreateJwtServiceMock();
            var dto = new LoginUserDto { Username = Username, Password = Password };
            var hasher = new PasswordHasher<User>();

            
            if (shouldSucceed || Username == "123")
            {
                var user = new User { Username = Username, Role = "User" };
                user.PasswordHash = hasher.HashPassword(user, "123"); 
                await context.Users.AddAsync(user);
                await context.SaveChangesAsync();
            }

            var handler = new LoginUserCommandHandler(context, hasher, mockJwtService.Object);
            var command = new LoginUserCommand(dto);

            var result = await handler.Handle(command, CancellationToken.None);

            if (shouldSucceed)
                Assert.IsType<Ok<TokenPairDto>>(result);
            else
                Assert.IsType<UnauthorizedHttpResult>(result);
        }


    }
}
