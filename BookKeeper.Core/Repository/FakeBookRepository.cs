namespace BookKeeper.Core.Repository;

// Keeps the books in a list in memory. Used by the domain tests and the API tests, so they do not need a database.
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
}
