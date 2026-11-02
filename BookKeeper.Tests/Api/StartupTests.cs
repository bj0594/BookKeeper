using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookKeeper.Tests.Api;

// Checks that the API refuses to start when its configuration is wrong.
public class StartupTests
{
    [Fact]
    public void Startup_WithoutJwtKey_Fails()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseSetting("<Section>:<Setting>", ""));

        // Act
        var exception = Record.Exception(() => factory.CreateClient());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("<Section>:<Setting>", exception.ToString());
    }
}
