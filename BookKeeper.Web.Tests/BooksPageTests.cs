using System.Net;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Web.Api;
using BookKeeper.Web.Components.Pages;

namespace BookKeeper.Web.Tests;

// Renders BooksPage with the fake client, so the tests need no API. The user is logged in unless a test says otherwise.
public class BooksPageTests : BunitContext
{
    private const string Librarian = "Librarian";
    private const string LibraryUser = "LibraryUser";

    private readonly FakeBooksClient _client = new();
    private readonly TokenStore _tokens = new() { Token = "test" };

    public BooksPageTests()
    {
        Services.AddSingleton<IBooksClient>(_client);
        Services.AddSingleton(_tokens);
    }

    // A book that is not loaned, for tests where the values don't matter.
    private BookDto AddBook()
    {
        var book = new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", false, null);
        _client.Items.Add(book);
        return book;
    }

    [Fact]
    public void BooksPage_TwoFromApi_ShowsBoth()
    {
        // Arrange
        _client.Items.Add(new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", false, null));
        _client.Items.Add(new BookDto(Guid.NewGuid(), "1984", "George Orwell", false, null));

        // Act
        var page = Render<BooksPage>();

        // Assert
        Assert.Equal(2, page.FindAll("li").Count);
    }

    [Fact]
    public void BooksPage_AddBook_SendsItToTheApi()
    {
        // Arrange
        _tokens.Role = Librarian;
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
        _tokens.Role = LibraryUser;
        var book = AddBook();
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
        _tokens.Role = Librarian;
        var book = AddBook();
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
        _tokens.Role = Librarian;
        _client.Items.Add(new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", true, Guid.NewGuid()));
        _client.Fail = new HttpRequestException("Book must be returned before deleting.");
        var page = Render<BooksPage>();

        // Act
        page.Find("li button.delete").Click();

        // Assert
        page.WaitForAssertion(() => Assert.Contains("Book must be returned before deleting.", page.Find(".error").TextContent));
    }

    [Fact]
    public void BooksPage_AddAfterRejectedDelete_ClearsMessage()
    {
        // Arrange
        _tokens.Role = Librarian;
        AddBook();
        _client.Fail = new HttpRequestException("Book must be returned before deleting.");
        var page = Render<BooksPage>();
        page.Find("li button.delete").Click();
        page.WaitForAssertion(() => Assert.NotEmpty(page.FindAll(".error")));

        // Act
        page.Find("#title").Change("1984");
        page.Find("#author").Change("George Orwell");
        page.Find("form").Submit();

        // Assert
        page.WaitForAssertion(() => Assert.Empty(page.FindAll(".error")));
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

    [Fact]
    public void BooksPage_TokenExpired_GoesToLogin()
    {
        // Arrange
        _client.LoadFail = new HttpRequestException("", null, HttpStatusCode.Unauthorized);
        var navigation = Services.GetRequiredService<NavigationManager>();

        // Act
        Render<BooksPage>();

        // Assert
        Assert.EndsWith("/login", navigation.Uri);
        Assert.False(_tokens.IsLoggedIn);
    }

    [Fact]
    public void BooksPage_LibraryUser_HidesAddAndDelete()
    {
        // Arrange
        _tokens.Role = LibraryUser;
        AddBook();

        // Act
        var page = Render<BooksPage>();

        // Assert
        Assert.Empty(page.FindAll("form"));
        Assert.Empty(page.FindAll("li button.delete"));
    }

    [Fact]
    public void BooksPage_Librarian_HidesLoan()
    {
        // Arrange
        _tokens.Role = Librarian;
        AddBook();

        // Act
        var page = Render<BooksPage>();

        // Assert
        Assert.Empty(page.FindAll("li button.loan"));
    }

    [Fact]
    public void BooksPage_LoanedBook_ShowsLoaned()
    {
        // Arrange
        _tokens.Role = Librarian;
        _client.Items.Add(new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", true, Guid.NewGuid()));

        // Act
        var page = Render<BooksPage>();

        // Assert
        Assert.Contains("Loaned", page.Find("li .status").TextContent);
    }
}
