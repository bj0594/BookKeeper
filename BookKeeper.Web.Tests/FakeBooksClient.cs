using BookKeeper.Web.Api;

namespace BookKeeper.Web.Tests;

// Answers like the API, without HTTP. The tests fill Items and Loans with what the page should get,
// and set LoadFail or Fail to make the lists or an action fail the way the API can.
public class FakeBooksClient : IBooksClient
{
    public Exception? LoadFail { get; set; }
    public Exception? Fail { get; set; }

    public List<BookDto> Items { get; } = [];
    public List<BookDto> Loans { get; } = [];
    public List<Guid> LoanedIds { get; } = [];
    public List<Guid> DeletedIds { get; } = [];
    public List<Guid> ReturnedIds { get; } = [];

    public Task<List<BookDto>> GetBooksAsync() =>
        LoadFail is null ? Task.FromResult(Items) : Task.FromException<List<BookDto>>(LoadFail);

    public Task<List<BookDto>> GetLoansAsync() =>
        LoadFail is null ? Task.FromResult(Loans) : Task.FromException<List<BookDto>>(LoadFail);

    public Task AddBookAsync(string title, string author)
    {
        Items.Add(new BookDto(Guid.NewGuid(), title, author, false, null));
        return Task.CompletedTask;
    }

    public Task LoanBookAsync(Guid id)
    {
        LoanedIds.Add(id);
        return Task.CompletedTask;
    }

    public Task DeleteBookAsync(Guid id)
    {
        if (Fail is not null) throw Fail;
        DeletedIds.Add(id);
        return Task.CompletedTask;
    }

    public Task ReturnBookAsync(Guid id)
    {
        if (Fail is not null) throw Fail;
        ReturnedIds.Add(id);
        return Task.CompletedTask;
    }
}
