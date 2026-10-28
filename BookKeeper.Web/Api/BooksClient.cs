using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace BookKeeper.Web.Api;

// Calls the API over HTTP. The pages only know IBooksClient.
public class BooksClient : IBooksClient
{
    private readonly HttpClient _http;

    public BooksClient(HttpClient http, TokenStore tokens)
    {
        _http = http;
        if (tokens.Token is not null)
        {
            _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokens.Token);
        }
    }

    public async Task<List<BookDto>> GetBooksAsync()
    {
        return await _http.GetFromJsonAsync<List<BookDto>>("books") ?? [];
    }

    public async Task AddBookAsync(string title, string author)
    {
        var response = await _http.PostAsJsonAsync("books", new { title, author });
        await EnsureSuccessAsync(response);
    }

    public async Task LoanBookAsync(Guid id)
    {
        var response = await _http.PatchAsync($"books/{id}/loan", null);
        await EnsureSuccessAsync(response);
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"books/{id}");
        await EnsureSuccessAsync(response);
    }

    // Throws with the API's own message, so the page can show it.
    private static async Task EnsureSuccessAsync(HttpResponseMessage response)
    {
        if (!response.IsSuccessStatusCode)
        {
            var message = await response.Content.ReadAsStringAsync();
            throw new HttpRequestException(message.Trim('"'), null, response.StatusCode);
        }
    }

    public async Task<List<BookDto>> GetLoansAsync()
    {
        return await _http.GetFromJsonAsync<List<BookDto>>("books/loans") ?? [];
    }

    public async Task ReturnBookAsync(Guid id)
    {
        var response = await _http.PatchAsync($"books/{id}/return", null);
        await EnsureSuccessAsync(response);
    }
}
