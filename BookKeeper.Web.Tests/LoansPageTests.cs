using Bunit;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Web.Api;
using BookKeeper.Web.Components.Pages;

namespace BookKeeper.Web.Tests;

// Renders LoansPage with the fake client, so the tests need no API.
public class LoansPageTests : BunitContext
{
    private readonly FakeBooksClient _client = new();

    public LoansPageTests()
    {
        Services.AddSingleton<IBooksClient>(_client);
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
}
