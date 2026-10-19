namespace BookKeeper.Core.Repository;

// Keeps the books in a list in memory. Used by the domain- and api-tests, and by the API until a database is added.
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
        // No need to do anything, since the book is already in the list and we are working with a reference type.
    }
}
