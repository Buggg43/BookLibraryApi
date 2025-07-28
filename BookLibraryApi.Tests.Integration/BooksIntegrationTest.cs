using BookLibraryApi;
using BookLibraryApi.Models.Dtos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;

public class BooksIntegrationTests : IntegrationTestBase
{
    public BooksIntegrationTests(WebApplicationFactory<Program> factory) : base(factory) { }

    [Fact]
    public async Task CreateBook_ShouldReturnCreated()
    {
        // Arrange – przygotowujemy dane do POST
        var book = new BookCreateDto
        {
            Title = "testTitle",
            Author = "testAuthor",
            Year = DateTime.UtcNow,
            Description = "Integration test book"
        };

        // Act – wysyłamy prawdziwe żądanie HTTP do API
        var response = await Client.PostAsJsonAsync("/books", book);

        // Assert – sprawdzamy odpowiedź
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var createdBook = await response.Content.ReadFromJsonAsync<BookReadDto>();
        createdBook.Should().NotBeNull();
        createdBook.Title.Should().Be("TestTitle");
    }
}
