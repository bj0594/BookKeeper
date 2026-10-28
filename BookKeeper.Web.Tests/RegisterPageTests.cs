using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Web.Api;
using BookKeeper.Web.Components.Pages;

namespace BookKeeper.Web.Tests;

// Renders RegisterPage with the fake auth client.
public class RegisterPageTests : BunitContext
{
    private readonly FakeAuthClient _auth = new();

    public RegisterPageTests()
    {
        Services.AddSingleton<IAuthClient>(_auth);
    }

    [Fact]
    public void RegisterPage_UsernameTaken_ShowsError()
    {
        // Arrange
        _auth.Succeeds = false;
        var page = Render<RegisterPage>();

        // Act
        page.Find("form").Submit();

        // Assert
        page.WaitForAssertion(() => Assert.NotNull(page.Find(".error")));
    }

    [Fact]
    public void RegisterPage_NewUser_GoesToLogin()
    {
        // Arrange
        _auth.Succeeds = true;
        var navigation = Services.GetRequiredService<NavigationManager>();
        var page = Render<RegisterPage>();

        // Act
        page.Find("form").Submit();

        // Assert
        page.WaitForAssertion(() => Assert.EndsWith("/login", navigation.Uri));
    }
}
