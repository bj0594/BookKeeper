using System.Net;
using BookKeeper.Web.Api;

namespace BookKeeper.Web.Tests;

// Tests how BooksClient turns the API's answers into errors the pages can show.
public class BooksClientTests
{
    [Fact]
    public async Task LoanBook_Forbidden_ThrowsWithMessage()
    {
        // Arrange
        var http = new HttpClient(new StubHandler(HttpStatusCode.Forbidden)) { BaseAddress = new Uri("http://localhost/") };
        var client = new BooksClient(http, new TokenStore());

        // Act
        var ex = await Assert.ThrowsAsync<HttpRequestException>(() => client.LoanBookAsync(Guid.NewGuid()));

        // Assert
        Assert.Equal("You are not allowed to do that.", ex.Message);
    }
}
