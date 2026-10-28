namespace BookKeeper.Web.Api;

// What the pages can ask the API for. BooksClient calls the API, and FakeBooksClient is used in the tests.
public interface IBooksClient
{
    Task<List<BookDto>> GetBooksAsync();
}
