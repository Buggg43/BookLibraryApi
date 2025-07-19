using BookLibraryApi.Data;
using BookLibraryApi.Models;
using BookLibraryApi.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace BookLibraryApi.Tests.Common
{
    public static class TestFactory
    {
        public static LibraryDbContext CreateContext(string dbName = "TestDb")
        {
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(dbName)
                .Options;
            
            return new LibraryDbContext(options);
        }

        public static User CreateTestUser(string username, string password, out PasswordHasher<User> hasher)
        {
            var user = new User { Username = username, Role = "User" };
            hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, password);
            return user;
        }

        public static Mock<JwtService> CreateJwtServiceMock()
        {
            var mock = new Mock<JwtService>();
            mock.Setup(x => x.GenerateToken(It.IsAny<User>()))
                .Returns("access_test");
            mock.Setup(x => x.GenerateRefreshToken(It.IsAny<User>()))
                .Returns(new RefreshToken { Token = "refresh_token" });

            return mock;
        }
    }
}
