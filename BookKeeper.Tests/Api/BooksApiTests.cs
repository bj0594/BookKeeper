using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookKeeper.Tests;

public class BooksApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BooksApiTests()
    {
        _factory = new WebApplicationFactory<Program>();
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    private record BookResponse(Guid Id, string Title, string Author, bool IsLoaned);

    [Fact]
    public async Task PostBook_ValidBook_ReturnsCreatedBook()
    {
        // Arrange
        var request = new { Title = "Test Book", Author = "Test Author" };

        // Act
        var response = await _client.PostAsJsonAsync("/books", new {request.Title, request.Author});

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var book = await response.Content.ReadFromJsonAsync<BookResponse>();
        Assert.Equal(request.Title, book?.Title);
        Assert.Equal(request.Author, book?.Author);
    }
}