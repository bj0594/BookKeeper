using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Core.Repository;
using BookKeeper.Core;

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
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", "Librarian");
        var created = await librarian.PostAsJsonAsync("/books", new { Title = "Test Book", Author = "Test Author" });
        return (await created.Content.ReadFromJsonAsync<BookResponse>())!;
    }

    [Fact]
    public async Task PostBooks_ValidBook_ReturnsCreatedBook()
    {
        // Arrange
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", "Librarian");
        var request = new { Title = "Test Book", Author = "Test Author" };

        // Act
        var response = await client.PostAsJsonAsync("/books", request);

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
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, "owner");
        await CreateBookAsync();

        // Act
        var response = await client.GetAsync("/books");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        Assert.Single(books!);
    }

    [Fact]
    public async Task PatchBooksLoan_ExistingBook_ReturnsNoContent()
    {
        // Arrange
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, "owner");
        var book = await CreateBookAsync();

        // Act
        var response = await client.PatchAsync($"/books/{book.Id}/loan", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PatchBooksReturn_LoanedBook_ReturnsNoContent()
    {
        // Arrange
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, "owner");
        var book = await CreateBookAsync();
        await client.PatchAsync($"/books/{book.Id}/loan", null);

        // Act
        var response = await client.PatchAsync($"/books/{book.Id}/return", null);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBooks_ExistingBook_ReturnsNoContent()
    {
        // Arrange
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", Roles.Librarian);
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, "owner");
        var book = await CreateBookAsync();

        // Act
        var response = await librarian.DeleteAsync($"/books/{book.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task PatchBooksLoan_AlreadyLoaned_ReturnsConflict()
    {
        // Arrange: ensures the book is already loaned before testing the conflict scenario
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, "owner");
        var book = await CreateBookAsync();
        await client.PatchAsync($"/books/{book.Id}/loan", null);

        // Act
        var response = await client.PatchAsync($"/books/{book.Id}/loan", null);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task PatchBooksReturn_NotLoaned_ReturnsConflict()
    {
        // Arrange: ensures the book is not loaned before testing the conflict scenario
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, "owner");
        var book = await CreateBookAsync();

        // Act
        var response = await client.PatchAsync($"/books/{book.Id}/return", null);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBooks_LoanedBook_ReturnsConflict()
    {
        // Arrange
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", Roles.Librarian);
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, "owner");
        var book = await CreateBookAsync();
        await client.PatchAsync($"/books/{book.Id}/loan", null);

        // Act
        var response = await librarian.DeleteAsync($"/books/{book.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task GetBooks_WithoutToken_ReturnsUnauthorized()
    {
        // Act
        var response = await _client.GetAsync("/books");

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task PostBooks_NotLibrarian_ReturnsForbidden()
    {
        // Arrange
        var user = await AuthHelper.CreateLoggedInClientAsync(_factory, "user");

        // Act
        var response = await user.PostAsJsonAsync("/books", new { Title = "Dune", Author = "Frank Herbert" });

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task DeleteBooks_NotLibrarian_ReturnsForbidden()
    {
        // Arrange
        var book = await CreateBookAsync();
        var user = await AuthHelper.CreateLoggedInClientAsync(_factory, "user");

        // Act
        var response = await user.DeleteAsync($"/books/{book.Id}");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatchBooksLoan_NotLibraryUser_ReturnsForbidden()
    {
        // Arrange: an object that exists, and a user with the other role
        var book = await CreateBookAsync();
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", "Librarian");

        // Act
        var response = await librarian.PatchAsync($"/books/{book.Id}/loan", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatchBooksReturn_NotLibraryUser_ReturnsForbidden()
    {
        // Arrange: an object that exists, and a user with the other role
        var book = await CreateBookAsync();
        var user = await AuthHelper.CreateLoggedInClientAsync(_factory, "user");
        await user.PatchAsync($"/books/{book.Id}/loan", null);
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", "Librarian");

        // Act
        var response = await librarian.PatchAsync($"/books/{book.Id}/return", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}
