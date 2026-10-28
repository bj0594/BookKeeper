using Bunit;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Web.Api;
using BookKeeper.Web.Components.Pages;
using Microsoft.AspNetCore.Components;

namespace BookKeeper.Web.Tests;

// Renders BooksPage with the fake client, so the tests need no API.
public class BooksPageTests : BunitContext
{
    private readonly FakeBooksClient _client = new ();
    private readonly TokenStore _tokens = new() { Token = "test" };

    public BooksPageTests()
    {
        Services.AddSingleton<IBooksClient>(_client);
        Services.AddSingleton(_tokens);
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

    [Fact]
    public void BooksPage_AddBook_SendsItToTheApi()
    {
        // Arrange
        var page = Render<BooksPage>();

        // Act
        page.Find("#title").Change("Dune");
        page.Find("#author").Change("Frank Herbert");
        page.Find("form").Submit();

        // Assert
        page.WaitForAssertion(() => Assert.Single(_client.Items));
    }

    [Fact]
    public void BooksPage_ClickLoan_SendsItToTheApi()
    {
        // Arrange
        var book = new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", false, null);
        _client.Items.Add(book);
        var page = Render<BooksPage>();

        // Act
        page.Find("li button.loan").Click();

        // Assert
        page.WaitForAssertion(() => Assert.Contains(book.Id, _client.LoanedIds));
    }

    [Fact]
    public void BooksPage_ClickDelete_SendsItToTheApi()
    {
        // Arrange
        var book = new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", false, null);
        _client.Items.Add(book);
        var page = Render<BooksPage>();

        // Act
        page.Find("li button.delete").Click();

        // Assert
        page.WaitForAssertion(() => Assert.Contains(book.Id, _client.DeletedIds));
    }

    [Fact]
    public void BooksPage_DeleteRejected_ShowsMessage()
    {
        // Arrange
        _client.Items.Add(new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", true, Guid.NewGuid()));
        _client.Fail = new HttpRequestException("Book must be returned before deleting.");
        var page = Render<BooksPage>();

        // Act
        page.Find("li button.delete").Click();

        // Assert
        page.WaitForAssertion(() => Assert.Contains("Book must be returned before deleting.", page.Find(".error").TextContent));
    }

    [Fact]
    public void BooksPage_NotLoggedIn_GoesToLogin()
    {
        // Arrange
        _tokens.Token = null;
        var navigation = Services.GetRequiredService<NavigationManager>();

        // Act
        Render<BooksPage>();

        // Assert
        Assert.EndsWith("/login", navigation.Uri);
    }
}
