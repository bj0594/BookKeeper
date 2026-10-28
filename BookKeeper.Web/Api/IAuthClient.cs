namespace BookKeeper.Web.Api;

// Registers and logs in through the API. AuthClient calls the API, and FakeAuthClient is used in the tests.
public interface IAuthClient
{
    Task<bool> LoginAsync(string username, string password);
    Task<bool> RegisterAsync(string username, string password, string role);
}
