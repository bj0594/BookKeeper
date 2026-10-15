namespace BookKeeper.Core;

// Keeps the library's books in memory and enforces the loan rules.
public class BookService
{
    private readonly List<Book> _books = [];

    // Creates a book with a new id, not loaned, and adds it to the library.
    public Book AddBook(string title, string author)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author
        };
        _books.Add(book);
        return book;
    }

    // Returns all books. The list is read-only, so books can only be added or removed through this service.
    public IReadOnlyList<Book> GetBooks()
    {
        return _books;
    }

    // Marks the book as loaned. Throws InvalidOperationException if it is already loaned or no book has the id.
    public void LoanBook(Guid bookId)
    {
        var book = GetBookById(bookId);
        if (book.IsLoaned)
        {
            throw new InvalidOperationException("Book is already loaned.");
        }
        book.IsLoaned = true;
    }

    // Marks the book as not loaned. Throws InvalidOperationException if it is not loaned or no book has the id.
    public void ReturnBook(Guid bookId)
    {
        var book = GetBookById(bookId);
        if (!book.IsLoaned)
        {
            throw new InvalidOperationException("Book is not loaned.");
        }
        book.IsLoaned = false;
    }

    // Removes the book from the library. Throws InvalidOperationException if no book has the id.
    public void DeleteBook(Guid bookId)
    {
        var book = GetBookById(bookId);
        _books.Remove(book);
    }

    private Book GetBookById(Guid bookId)
    {
        return _books.First(b => b.Id == bookId);
    }
}
