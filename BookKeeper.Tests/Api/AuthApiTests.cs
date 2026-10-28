using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using BookKeeper.Core;
using BookKeeper.Core.Repository;

namespace BookKeeper.Tests.Api;

// Contract tests for /auth in Documentation/api.md.
public class AuthApiTests : IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private const string Username = "owner";
    private const string Password = "Test-password-1";
    private const string DefaultRole = Roles.LibraryUser;

    public AuthApiTests()
    {
        _factory = new BookKeeperFactory();
        _client = _factory.CreateClient();
    }

    public void Dispose() => _factory.Dispose();
    private record LoginResponse(string Token);
    private record ClaimResponse(string Type, string Value);

    [Fact]
    public async Task PostAuthRegister_NewUser_ReturnsCreated()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", new { Username, Password, Role = DefaultRole });

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
    }

    [Fact]
    public async Task PostAuthRegister_TakenUsername_ReturnsConflict()
    {
        // Arrange
        var credentials = new { Username, Password, Role = DefaultRole };
        await _client.PostAsJsonAsync("/auth/register", credentials);

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", credentials);

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task PostAuthRegister_NewUser_StoresOnlyHash()
    {
        // Act
        await _client.PostAsJsonAsync("/auth/register", new { Username, Password, Role = DefaultRole });

        // Assert
        var user = _factory.Services.GetRequiredService<IUserRepository>().GetByUsername(Username);
        Assert.NotEqual(Password, user!.PasswordHash);
        Assert.StartsWith("$2", user.PasswordHash);
    }

    [Fact]
    public async Task PostAuthLogin_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var credentials = new { Username, Password, Role = DefaultRole };
        await _client.PostAsJsonAsync("/auth/register", credentials);

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", credentials);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        Assert.False(string.IsNullOrEmpty(body!.Token));
    }

    [Fact]
    public async Task PostAuthLogin_WrongPassword_ReturnsUnauthorized()
    {
        // Arrange
        await _client.PostAsJsonAsync("/auth/register", new { Username, Password, Role = DefaultRole });

        // Act
        var response = await _client.PostAsJsonAsync("/auth/login", new { Username, Password = "Wrong-password" });

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetAuthMe_WithToken_ReturnsUsername()
    {
        // Arrange
        var client = await AuthHelper.CreateLoggedInClientAsync(_factory, Username);

        // Act
        var response = await client.GetAsync("/auth/me");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var claims = await response.Content.ReadFromJsonAsync<List<ClaimResponse>>();
        Assert.Contains(claims!, c => c.Value == Username);
    }

    [Fact]
    public async Task PostAuthRegister_InvalidRole_ReturnsBadRequest()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", new { Username, Password, Role = "Admin" });

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task PostAuthRegister_SecondLibrarian_ReturnsConflict()
    {
        // Arrange
        await _client.PostAsJsonAsync("/auth/register", new { Username = "first", Password, Role = Roles.Librarian });

        // Act
        var response = await _client.PostAsJsonAsync("/auth/register", new { Username = "second", Password, Role = Roles.Librarian });

        // Assert
        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }
}
