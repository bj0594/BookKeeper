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
        var book = _books.First(i => i.Id == bookId);
        book.IsLoaned = true;
    }
}