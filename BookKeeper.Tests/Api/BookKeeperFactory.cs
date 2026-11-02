using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Core.Repository;
using Microsoft.Extensions.Configuration;

namespace BookKeeper.Tests.Api;

// Starts the API in memory with the fake repositories, so the API tests need no database.
// xUnit creates each test class once per test, so each test gets its own API with empty lists.
public class BookKeeperFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
                config.AddInMemoryCollection(new Dictionary<string, string?> { ["Jwt:Key"] = "test-key-for-the-api-tests-only-0123456789" }));

        builder.ConfigureTestServices(services =>
        {
            services.AddSingleton<IBookRepository, FakeBookRepository>();
            services.AddSingleton<IUserRepository, FakeUserRepository>();
        });
    }
}
