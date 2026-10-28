using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using BookKeeper.Core;

namespace BookKeeper.Tests.Api;

// Contract tests for /books in Documentation/api.md.
public class BooksApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;

    public BooksApiTests()
    {
        _factory = new BookKeeperFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();

    private record BookResponse(Guid Id, string Title, string Author, bool IsLoaned);

    // Adds a book as the librarian, so each test can start with a book that exists.
    private async Task<BookResponse> CreateBookAsync()
    {
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", Roles.Librarian);
        var created = await librarian.PostAsJsonAsync("/books", new { Title = "Test Book", Author = "Test Author" });
        return (await created.Content.ReadFromJsonAsync<BookResponse>())!;
    }

    [Fact]
    public async Task PostBooks_ValidBook_ReturnsCreatedBook()
    {
        // Arrange
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", Roles.Librarian);
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
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", Roles.Librarian);

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
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", Roles.Librarian);

        // Act
        var response = await librarian.PatchAsync($"/books/{book.Id}/return", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetBooks_LibraryUser_DoesNotShowLoanedBooks()
    {
        // Arrange: a loaned book
        var user = await AuthHelper.CreateLoggedInClientAsync(_factory, "user");
        var book = await CreateBookAsync();
        await user.PatchAsync($"/books/{book.Id}/loan", null);

        // Act
        var response = await user.GetAsync("/books");

        // Assert
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        Assert.Empty(books!);
    }

    [Fact]
    public async Task GetBooksLoans_TwoUsers_ReturnsOnlyOwn()
    {
        // Arrange: two users, and each loans their own book
        var owner = await AuthHelper.CreateLoggedInClientAsync(_factory, "owner");
        var other = await AuthHelper.CreateLoggedInClientAsync(_factory, "other");
        var ownBook = await CreateBookAsync();
        var otherBook = await CreateBookAsync();
        await owner.PatchAsync($"/books/{ownBook.Id}/loan", null);
        await other.PatchAsync($"/books/{otherBook.Id}/loan", null);

        // Act
        var response = await owner.GetAsync("/books/loans");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        var found = Assert.Single(books!);
        Assert.Equal(ownBook.Id, found.Id);
    }

    [Fact]
    public async Task GetBooksLoans_NotLibraryUser_ReturnsForbidden()
    {
        // Arrange: a user with the other role
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", Roles.Librarian);

        // Act
        var response = await librarian.GetAsync("/books/loans");

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task PatchBooksReturn_LoanedByOtherUser_ReturnsForbidden()
    {
        // Arrange
        var book = await CreateBookAsync();
        var owner = await AuthHelper.CreateLoggedInClientAsync(_factory, "owner");
        var other = await AuthHelper.CreateLoggedInClientAsync(_factory, "other");
        await owner.PatchAsync($"/books/{book.Id}/loan", null);

        // Act
        var response = await other.PatchAsync($"/books/{book.Id}/return", null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }

    [Fact]
    public async Task GetBooks_Librarian_ShowsLoanedBooks()
    {
        // Arrange
        var user = await AuthHelper.CreateLoggedInClientAsync(_factory, "user");
        var librarian = await AuthHelper.CreateLoggedInClientAsync(_factory, "librarian", Roles.Librarian);
        var book = await CreateBookAsync();
        await user.PatchAsync($"/books/{book.Id}/loan", null);

        // Act
        var response = await librarian.GetAsync("/books");

        // Assert
        var books = await response.Content.ReadFromJsonAsync<List<BookResponse>>();
        var found = Assert.Single(books!);
        Assert.True(found.IsLoaned);
    }
}
