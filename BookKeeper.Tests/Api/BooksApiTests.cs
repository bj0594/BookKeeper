using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Core.Repository;

namespace BookKeeper.Tests.Api;

public class BooksApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BooksApiTests()
    {
        // xUnit creates this class once per test, so each test gets its own API with an empty book list.
        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.ConfigureTestServices(services =>
            {
                // Use the fake repository for testing, so we don't need a database.
                services.AddSingleton<IBookRepository, FakeBookRepository>();
                services.AddSingleton<IUserRepository, FakeUserRepository>();
            }));
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    private record BookResponse(Guid Id, string Title, string Author, bool IsLoaned);

    private async Task<BookResponse> CreateBookAsync()
    {
        var response = await _client.PostAsJsonAsync("/books", new { Title = "Test Book", Author = "Test Author" });
        return (await response.Content.ReadFromJsonAsync<BookResponse>())!;
    }

    [Fact]
    public async Task PostBooks_ValidBook_ReturnsCreatedBook()
    {
        // Arrange
        var request = new { Title = "Test Book", Author = "Test Author" };

        // Act
        var response = await _client.PostAsJsonAsync("/books", request);

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
        // Arrange: ensures the book is already loaned before testing the conflict scenario
        var book = await CreateBookAsync();
        await _client.PatchAsync($"/books/{book.Id}/loan", null);

        // Act
        var response = await _client.PatchAsync($"/books/{book.Id}/loan", null);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task PatchBooksReturn_NotLoaned_ReturnsConflict()
    {
        // Arrange: ensures the book is not loaned before testing the conflict scenario
        var book = await CreateBookAsync();

        // Act
        var response = await _client.PatchAsync($"/books/{book.Id}/return", null);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBooks_LoanedBook_ReturnsConflict()
    {
        // Arrange
        var book = await CreateBookAsync();
        await _client.PatchAsync($"/books/{book!.Id}/loan", null);

        // Act
        var response = await _client.DeleteAsync($"/books/{book!.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
