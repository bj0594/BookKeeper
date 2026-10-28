using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Web.Api;
using BookKeeper.Web.Components.Pages;

namespace BookKeeper.Web.Tests;

// Renders LogoutPage with a logged-in user.
public class LogoutPageTests : BunitContext
{
    private readonly TokenStore _tokens = new() { Token = "test", Role = "LibraryUser" };

    public LogoutPageTests()
    {
        Services.AddSingleton(_tokens);
    }

    [Fact]
    public void LogoutPage_LoggedIn_ForgetsTokenAndGoesToLogin()
    {
        // Arrange
        var navigation = Services.GetRequiredService<NavigationManager>();

        // Act
        Render<LogoutPage>();

        // Assert
        Assert.False(_tokens.IsLoggedIn);
        Assert.Null(_tokens.Role);
        Assert.EndsWith("/login", navigation.Uri);
    }
}
