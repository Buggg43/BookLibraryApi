using AutoMapper;
using BookLibraryApi.Data;
using BookLibraryApi.Mapper;
using BookLibraryApi.Models;
using BookLibraryApi.Services;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Security.Claims;

namespace BookLibraryApi.Tests.Common
{
    public static class TestFactory
    {
        public static LibraryDbContext CreateContext(string? dbName = null)
        {
            var name = dbName ?? Guid.NewGuid().ToString(); // zawsze unikalna baza
            var options = new DbContextOptionsBuilder<LibraryDbContext>()
                .UseInMemoryDatabase(name)
                .Options;

            return new LibraryDbContext(options);
        }
        public static IMapper CreateMapper(Microsoft.Extensions.Logging.ILoggerFactory loggerFactory)
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, loggerFactory);
            config.AssertConfigurationIsValid();
            return config.CreateMapper();
        }

        public static User CreateTestUser(string username,string password, out PasswordHasher<User> hasher, int? id = null)
        {
            var user = new User { Id = id ?? 1, Username = username, Role = "User" };
            hasher = new PasswordHasher<User>();
            user.PasswordHash = hasher.HashPassword(user, password);
            return user;
        }
        public static RefreshToken CreateRefreshToken(User user, IJwtService jwt)
        {
            var token = jwt.GenerateRefreshToken(user);
            return token;
        }

        public static Mock<IJwtService> CreateJwtServiceMock()
        {
            var mock = new Mock<IJwtService>();
            mock.Setup(x => x.GenerateToken(It.IsAny<User>())).Returns("access_token_test");
            mock.Setup(x => x.GenerateRefreshToken(It.IsAny<User>()))
                .Returns((User user) => new RefreshToken
                {
                    UserId = user.Id,
                    Token = "refresh_token_test",
                    Expires = DateTime.UtcNow.AddDays(7),
                    isRevoked = false,
                    User = user
                });

            return mock;
        }
        public static Book CreateTestBook(int id, int userId, string title = "Default Title", bool isRead = false)
        {
            return new Book
            {
                Id = id,
                Title = title,
                Author = "testAuthor",
                Year = DateTime.UtcNow,
                isRead = isRead,
                isFavorite = false,
                Description = "Test",
                UserId = userId
            };
        }
        public static async Task AddBooksAsync(LibraryDbContext context, params Book[] books)
        {
            await context.Books.AddRangeAsync(books);
            await context.SaveChangesAsync();
        }
        public static async Task AddUsersAsync(LibraryDbContext context, params User[] users)
        {
            await context.Users.AddRangeAsync(users);
            await context.SaveChangesAsync();
        }


        public static ClaimsPrincipal CreateClaimsPrincipal(string username, int? id = null, string? role = "User")
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.NameIdentifier, (id ?? 1).ToString()),
                new Claim(ClaimTypes.Role, role ?? "User") 
            };
            var identity = new ClaimsIdentity(claims, "TestAuth");
            return new ClaimsPrincipal(identity);
        }
        public static IConfiguration CreateConfiguration()
        {
            var dict = new Dictionary<string, string>
            {
                { "Jwt:Key", "your_secret_key_123456789012345" },
                { "Jwt:Issuer", "test_issuer" },
                { "Jwt:ExpireMinutes", "60" }
            };

            return new ConfigurationBuilder()
                .AddInMemoryCollection(dict)
                .Build();
        }

    }
}
