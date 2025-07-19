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

        [Fact]
        public async Task Handle_ShouldReturnTokenPair_WhenCredentialsAreValid()
        {
            var context = TestFactory.CreateContext();
            var user = TestFactory.CreateTestUser("123", "123", out var hasher);
            var mockJwtService = TestFactory.CreateJwtServiceMock();
            context.Users.Add(user); context.SaveChanges();



            var handler = new LoginUserCommandHandler(context, hasher, mockJwtService.Object);


            var inputForTest= new List<LoginUserDto>();
            inputForTest.Add(new LoginUserDto { Password = "123", Username = "123" });

            inputForTest.Add(new LoginUserDto { Password = "1234", Username = "123" });

            foreach (var input in inputForTest) {
                var command = new LoginUserCommand(input);
                var result = await handler.Handle(command, CancellationToken.None);

                var okResult = result as Ok<TokenPairDto>;

                Assert.NotNull(okResult);
                Assert.Equal("access_test", okResult.Value.AccessToken);
                Assert.Equal("refresh_test", okResult.Value.RefreshToken);
            }

        }
        [Fact]
        public async Task Handle_ShouldReturnUnauthorized_WhenPasswordIsInvalid()
        {
            // Arrange
            var context = TestFactory.CreateContext();
            var user = TestFactory.CreateTestUser("123", "123", out var hasher);
            var mockJwtService = TestFactory.CreateJwtServiceMock();
            context.Users.Add(user); context.SaveChanges();

            var handler = new LoginUserCommandHandler(context, hasher, mockJwtService.Object);
            var command = new LoginUserCommand(new LoginUserDto { Username = "123", Password = "wrongpass" });

            // Act
            var result = await handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.IsType<UnauthorizedHttpResult>(result);
        }

    }
}
