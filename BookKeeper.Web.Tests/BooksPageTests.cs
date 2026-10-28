using Bunit;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Web.Api;
using BookKeeper.Web.Components.Pages;

namespace BookKeeper.Web.Tests;

// Renders BooksPage with the fake client, so the tests need no API.
public class BooksPageTests : BunitContext
{
    private readonly FakeBooksClient _client = new ();

    public BooksPageTests()
    {
        Services.AddSingleton<IBooksClient>(_client);
    }

    [Fact]
    public void BooksPage_TwoFromApi_ShowsBoth()
    {
        // Arrange
        _client.Items.Add(new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", false, null));
        _client.Items.Add(new BookDto(Guid.NewGuid(), "1984", "George Orwell", false, null));

        // Act
        var page = Render <BooksPage> ();

        // Assert
        Assert.Equal(2, page.FindAll("li").Count);
    }
}
