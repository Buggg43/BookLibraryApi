using BookLibraryApi.Models.Dtos;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;

public class BooksIntegrationTests : IntegrationTestBase
{
    public BooksIntegrationTests(CustomWebApplicationFactory factory) : base(factory) { }

    [Fact]
    public async Task CreateBook_ShouldReturnCreated()
    {
        var registerDto = new RegisterUserDto
        {
            Username = $"bookuser{Guid.NewGuid():N}".Substring(0, 10),
            Password = "password123"
        };

        var registerResponse = await Client.PostAsJsonAsync("/api/auth/register", registerDto);
        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var loginResponse = await Client.PostAsJsonAsync("/api/auth/login", new LoginUserDto
        {
            Username = registerDto.Username,
            Password = registerDto.Password
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var tokens = await loginResponse.Content.ReadFromJsonAsync<TokenPairDto>();
        tokens.Should().NotBeNull();

        Client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", tokens!.AccessToken);

        var book = new BookCreateDto
        {
            Title = "testTitle",
            Author = "testAuthor",
            Year = DateTime.UtcNow,
            Description = "Integration test book"
        };

        var response = await Client.PostAsJsonAsync("/api/books", book);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdBook = await response.Content.ReadFromJsonAsync<BookReadDto>();
        createdBook.Should().NotBeNull();
        createdBook!.Title.Should().Be("testTitle");
    }
}
