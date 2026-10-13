namespace BookKeeper.Core;

public class BookService
{
    private readonly List<Book> _books = new();
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
}