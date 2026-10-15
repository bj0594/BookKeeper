using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.OpenApi;

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

    private async Task<BookResponse> CreateBookAsync()
    {
        var response = await _client.PostAsJsonAsync("/books", new { title = "Test Book", author = "Test Author" });
        return (await response.Content.ReadFromJsonAsync<BookResponse>())!;
    }

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

    [Fact]
    public async Task GetBooks_AfterBookAdded_ReturnsOkWithList()
    {
        // Arrange
        await CreateBookAsync();

        // Act
        var response = await _client.GetAsync("/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        Assert.Single(books!);
    }

    [Fact]
    public async Task PatchBooksLoan_ExistingBook_ReturnsNoContent()
    {
        // Arrange
        var book = await CreateBookAsync();

        // Act
        var response = await _client.PatchAsync($"/books/{book.Id}/loan", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PatchBooksReturn_LoanedBook_ReturnsNoContent()
    {
        // Arrange
        var book = await CreateBookAsync();
        await _client.PatchAsync($"/books/{book.Id}/loan", null);

        // Act
        var response = await _client.PatchAsync($"/books/{book.Id}/return", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBooks_ExistingBook_ReturnsNoContent()
    {
        // Arrange
        var book = await CreateBookAsync();

        // Act
        var response = await _client.DeleteAsync($"/books/{book.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PatchBooksLoan_AlreadyLoaned_ReturnsConflict()
    {
        // Arrange: put the object in the state that breaks the rule (loaned)
        var book = await CreateBookAsync();
        await _client.PatchAsync($"/books/{book.Id}/loan", null);

        // Act
        var response = await _client.PatchAsync($"/books/{book.Id}/loan", null);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}