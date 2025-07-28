using BookLibraryApi.Models.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace BookLibraryApi.Tests.Integration
{
    public class AuthIntegrationTests : IntegrationTestBase
    {
        public AuthIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

        private async Task<TokenPairDto> RegisterAndLoginAsync(string username)
        {
            await Client.PostAsJsonAsync("/auth/register", new RegisterUserDto
            {
                Username = username,
                Password = "password123"
            });

            var response = await Client.PostAsJsonAsync("/auth/login", new LoginUserDto
            {
                Username = username,
                Password = "password123"
            });

            return await response.Content.ReadFromJsonAsync<TokenPairDto>() ?? throw new Exception("Login failed");
        }

        [Fact]
        public async Task Register_ShouldReturnCreated()
        {
            var registerDto = new RegisterUserDto
            {
                Username = "test_user",
                Password = "password123"
            };

            var response = await Client.PostAsJsonAsync("/auth/register", registerDto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Login_ShouldReturnJwtTokens()
        {
            var registerDto = new RegisterUserDto { Username = "login_user", Password = "password123" };
            await Client.PostAsJsonAsync("/auth/register", registerDto);

            var loginDto = new LoginUserDto { Username = "login_user", Password = "password123" };
            var response = await Client.PostAsJsonAsync("/auth/login", loginDto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var tokens = await response.Content.ReadFromJsonAsync<TokenPairDto>();
            tokens.Should().NotBeNull();
            tokens.AccessToken.Should().NotBeNullOrEmpty();
            tokens.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnNoContent()
        {
            var tokens = await RegisterAndLoginAsync("delete_user");
            Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var bookDto = new BookCreateDto
            {
                Title = "BookToDelete",
                Author = "Tester",
                Year = DateTime.UtcNow,
                Description = "DeleteMe"
            };

            var createResponse = await Client.PostAsJsonAsync("/books", bookDto);
            var createdBook = await createResponse.Content.ReadFromJsonAsync<BookReadDto>();

            var deleteResponse = await Client.DeleteAsync($"/books/{createdBook.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens()
        {
            var tokens = await RegisterAndLoginAsync("refresh_user");

            var refreshDto = new RefreshTokenRequestDto
            {
                token = tokens.RefreshToken
            };

            var response = await Client.PostAsJsonAsync("/auth/refresh", refreshDto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var newTokens = await response.Content.ReadFromJsonAsync<TokenPairDto>();
            newTokens.Should().NotBeNull();
            newTokens.AccessToken.Should().NotBeNullOrEmpty();
            newTokens.RefreshToken.Should().NotBeNullOrEmpty();
        }
    }
}
