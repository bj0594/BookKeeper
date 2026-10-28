using Bunit;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Web.Api;
using BookKeeper.Web.Components.Pages;

namespace BookKeeper.Web.Tests;

// Renders LoginPage with the fake auth client.
public class LoginPageTests : BunitContext
{
    private readonly FakeAuthClient _auth = new();

    public LoginPageTests()
    {
        Services.AddSingleton<IAuthClient>(_auth);
    }

    [Fact]
    public void LoginPage_WrongPassword_ShowsError()
    {
        // Arrange
        _auth.Succeeds = false;
        var page = Render<LoginPage>();

        // Act
        page.Find("form").Submit();

        // Assert
        page.WaitForAssertion(() => Assert.NotNull(page.Find(".error")));
    }
}
