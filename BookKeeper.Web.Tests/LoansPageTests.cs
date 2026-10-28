using Bunit;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Web.Api;
using BookKeeper.Web.Components.Pages;
using Microsoft.AspNetCore.Components;

namespace BookKeeper.Web.Tests;

// Renders LoansPage with the fake client, so the tests need no API.
public class LoansPageTests : BunitContext
{
    private readonly FakeBooksClient _client = new();
    private readonly TokenStore _tokens = new() { Token = "test" };
    public LoansPageTests()
    {
        Services.AddSingleton<IBooksClient>(_client);
        Services.AddSingleton(_tokens);
    }

    [Fact]
    public void LoansPage_TwoLoans_ShowsBoth()
    {
        // Arrange
        _client.Loans.Add(new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", true, Guid.NewGuid()));
        _client.Loans.Add(new BookDto(Guid.NewGuid(), "1984", "George Orwell", true, Guid.NewGuid()));

        // Act
        var page = Render<LoansPage>();

        // Assert
        Assert.Equal(2, page.FindAll("li").Count);
    }

    [Fact]
    public void LoansPage_ClickReturn_SendsItToTheApi()
    {
        // Arrange
        var book = new BookDto(Guid.NewGuid(), "Dune", "Frank Herbert", true, Guid.NewGuid());
        _client.Loans.Add(book);
        var page = Render<LoansPage>();

        // Act
        page.Find("li button.return").Click();

        // Assert
        page.WaitForAssertion(() => Assert.Contains(book.Id, _client.ReturnedIds));
    }

    [Fact]
    public void LoansPage_NotLoggedIn_GoesToLogin()
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
