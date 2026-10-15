namespace BookKeeper.Core;

public class BookService
{
    private readonly List<Book> _books = [];

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

    public List<Book> GetBooks()
    {
        return _books;
    }

    public void LoanBook(Guid bookId)
    {
        var book = GetBookById(bookId);
        if (book.IsLoaned)
        {
            throw new InvalidOperationException("Book is already loaned.");
        }
        book.IsLoaned = true;
    }

    public void ReturnBook(Guid bookId)
    {
        var book = GetBookById(bookId);
        if (!book.IsLoaned)
        {
            throw new InvalidOperationException("Book is not loaned.");
        }
        book.IsLoaned = false;
    }

    public void DeleteBook(Guid bookId)
    {
        var book = GetBookById(bookId);
        _books.Remove(book);
    }
    
    private Book GetBookById(Guid bookId)
    {
        return _books.First(i => i.Id == bookId);
    }
}