using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookKeeper.Tests;

public class BooksApiTests
{
  private readonly WebApplicationFactory<Program> _factory;
  private readonly HttpClient _client;

  public BooksApiTests()
  {
    _factory = new WebApplicationFactory<Program>();
    _client = _factory.CreateClient();
  }

    [Fact]
  public void Dispose() => _factory.Dispose();

  //The contract as the test sees it. Don't change this unless you want to break the tests.
  private record BookResponse(Guid Id, string Title, string Author, bool IsLoaned);

  [Fact]
  public async Task PostBook_ValidBook_ReturnsCreatedBook()
  {
    // Act
    var response = await _client.PostAsJsonAsync("/books", new { Title = "Test Book", Author = "Test Author" });

    // Assert
    Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    var book = await response.Content.ReadFromJsonAsync<BookResponse>();
    Assert.Equal("Test Book", book?.Title);
    Assert.Equal("Test Author", book?.Author);
  }
}