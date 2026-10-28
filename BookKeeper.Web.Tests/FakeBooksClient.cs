using BookKeeper.Web.Api;

namespace BookKeeper.Web.Tests;

// Answers like the API, without HTTP. The tests fill Items with the books the page should get.
public class FakeBooksClient : IBooksClient
{
    public List<BookDto> Items { get; } = [];

    public Task<List<BookDto>> GetBooksAsync() => Task.FromResult(Items);
}
