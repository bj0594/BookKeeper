namespace BookKeeper.Core.Repository;

public class FakeBookRepository : IBookRepository
{
    private readonly List<Book> _books = [];

    public void Add(Book book)
    {
        _books.Add(book);
    }

    public IReadOnlyList<Book> GetAll()
    {
        return _books;
    }

    public void Delete(Book book)
    {
        _books.Remove(book);
    }

    public void Update(Book book)
    {
        // Nothing to do: Book is a reference type, so the book in the list is already the changed object.
    }

    public IReadOnlyList<Book> GetLoanedBy(Guid userId) => _books.Where(b => b.LoanedByUserId == userId).ToList();
}
