using BookLibraryApi.Models.Dtos;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

namespace BookLibraryApi.Tests.Integration
{
    public class AuthIntegrationTests : IntegrationTestBase
    {
        public AuthIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

        private async Task<TokenPairDto> RegisterAndLoginAsync(string? username = null)
        {
            username ??= $"testuser{Guid.NewGuid():N}".Substring(0, 10); 

            var registerDto = new RegisterUserDto
            {
                Username = username,
                Password = "password123"
            };

            var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerDto);
            registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            registerResponse.EnsureSuccessStatusCode();


            var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", new LoginUserDto
            {
                Username = username,
                Password = "password123"
            });
            loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            loginResponse.EnsureSuccessStatusCode();

            return await loginResponse.Content.ReadFromJsonAsync<TokenPairDto>()
                   ?? throw new Exception("Invalid JSON returned from /api/auth/login");
        }


        [Fact]
        public async Task Register_ShouldReturnCreated()
        {
            var registerDto = new RegisterUserDto
            {
                Username = $"testuser{Guid.NewGuid():N}".Substring(0, 10),
                Password = "password123"
            };

            var response = await Client.PostAsJsonAsync("/api/auth/register", registerDto);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
        }

        [Fact]
        public async Task Login_ShouldReturnJwtTokens()
        {
            var tokens = await RegisterAndLoginAsync();

            tokens.Should().NotBeNull();
            tokens.AccessToken.Should().NotBeNullOrEmpty();
            tokens.RefreshToken.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task DeleteBook_ShouldReturnNoContent()
        {
            var tokens = await RegisterAndLoginAsync();

            Client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", tokens.AccessToken);

            var bookDto = new BookCreateDto
            {
                Title = "BookToDelete",
                Author = "Tester",
                Year = DateTime.UtcNow,
                Description = "DeleteMe"
            };

            var createResponse = await Client.PostAsJsonAsync("/api/books", bookDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var createdBook = await createResponse.Content.ReadFromJsonAsync<BookReadDto>();
            var deleteResponse = await Client.DeleteAsync($"/api/books/{createdBook!.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task RefreshToken_ShouldReturnNewTokens()
        {
            var tokens = await RegisterAndLoginAsync();

            var refreshDto = new RefreshTokenRequestDto
            {
                token = tokens.RefreshToken
            };

            var response = await Client.PutAsJsonAsync("/api/auth/refresh", refreshDto);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var newTokens = await response.Content.ReadFromJsonAsync<TokenPairDto>();
            newTokens.Should().NotBeNull();
            newTokens.AccessToken.Should().NotBeNullOrEmpty();
            newTokens.RefreshToken.Should().NotBeNullOrEmpty();
        }
    }
}
