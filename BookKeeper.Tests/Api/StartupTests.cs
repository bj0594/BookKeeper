using Microsoft.Extensions.Configuration;

namespace BookKeeper.Tests.Api;

// Checks that the API refuses to start when its configuration is wrong.
public class StartupTests
{
    [Fact]
    public void Startup_WithoutJwtKey_Fails()
    {
        // Arrange
        using var factory = new BookKeeperFactory().WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?> { ["Jwt:Key"] = "" })));

        // Act
        var exception = Record.Exception(() => factory.CreateClient());

        // Assert
        Assert.NotNull(exception);
        Assert.Contains("Jwt:Key", exception.ToString());
    }
}
