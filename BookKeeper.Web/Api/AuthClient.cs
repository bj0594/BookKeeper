using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BookKeeper.Web.Api;

// Registers and logs in through the API, and keeps the token and the role in TokenStore.
public class AuthClient : IAuthClient
{
    private readonly HttpClient _http;
    private readonly TokenStore _tokens;

    public AuthClient(HttpClient http, TokenStore tokens)
    {
        _http = http;
        _tokens = tokens;
    }

    public async Task<bool> LoginAsync(string username, string password)
    {
        var response = await _http.PostAsJsonAsync("auth/login", new { username, password });
        if (!response.IsSuccessStatusCode)
        {
            return false;
        }

        var body = await response.Content.ReadFromJsonAsync<LoginResponse>();
        _tokens.Token = body!.Token;

        // The role decides which buttons the pages show. /auth/me returns the claims in the token.
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _tokens.Token);
        var claims = await _http.GetFromJsonAsync<List<ClaimResponse>>("auth/me");
        _tokens.Role = claims?.FirstOrDefault(c => c.Type == "role" || c.Type.EndsWith("/role"))?.Value;
        return true;
    }

    public async Task<bool> RegisterAsync(string username, string password, string role)
    {
        var response = await _http.PostAsJsonAsync("auth/register", new { username, password, role });
        return response.IsSuccessStatusCode;
    }

    private record LoginResponse(string Token);
    private record ClaimResponse(string Type, string Value);
}
