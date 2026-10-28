using System.Net.Http.Json;

namespace BookKeeper.Web.Api;

// Calls the API over HTTP. The pages only know IBooksClient.
public class BooksClient : IBooksClient
{
    private readonly HttpClient _http;

    public BooksClient(HttpClient http)
    {
        _http = http;
    }

    public async Task<List<BookDto>> GetBooksAsync()
    {
        return await _http.GetFromJsonAsync<List<BookDto>>("books") ?? [];
    }

    public async Task AddBookAsync(string title, string author)
    {
        var response = await _http.PostAsJsonAsync("books", new { title, author });
        response.EnsureSuccessStatusCode();
    }

    public async Task LoanBookAsync(Guid id)
    {
        var response = await _http.PatchAsync($"books/{id}/loan", null);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteBookAsync(Guid id)
    {
        var response = await _http.DeleteAsync($"books/{id}");
        response.EnsureSuccessStatusCode();
    }
}
