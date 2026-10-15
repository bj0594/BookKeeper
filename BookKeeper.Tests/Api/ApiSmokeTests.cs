using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BookKeeper.Tests.Api;

public class ApiSmokeTests
{
    [Fact]
    public async Task UnknownRoute_ReturnsNotFound()
    {
        // Arrange
        using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();

        // Act
        var response = await client.GetAsync("/does-not-exist");

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}
