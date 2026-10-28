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
}
