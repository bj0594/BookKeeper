using BookKeeper.Web.Api;

namespace BookKeeper.Web.Tests;

// Answers like the API, without HTTP. The tests fill Items with the books the page should get.
public class FakeBooksClient : IBooksClient
{
    public Exception? Fail { get; set; }
    public List<BookDto> Items { get; } = [];

    public Task<List<BookDto>> GetBooksAsync() => Task.FromResult(Items);
    public Task AddBookAsync(string title, string author)
    {
        Items.Add(new BookDto(Guid.NewGuid(), title, author, false, null));
        return Task.CompletedTask;
    }

    public List<Guid> LoanedIds { get; } = [];

    public Task LoanBookAsync(Guid id)
    {
        LoanedIds.Add(id);
        return Task.CompletedTask;
    }

    public List<Guid> DeletedIds { get; } = [];

    public Task DeleteBookAsync(Guid id)
    {
        if (Fail is not null) throw Fail;
        DeletedIds.Add(id);
        return Task.CompletedTask;
    }
}
