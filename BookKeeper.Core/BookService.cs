using BookKeeper.Core.Repository;

namespace BookKeeper.Core;

// Enforces the loan rules. The books are stored through IBookRepository.
public class BookService
{
    private readonly IBookRepository _repository;
    private readonly ILoanRepository _loanRepository;

    public BookService(IBookRepository repository, ILoanRepository loanRepository)
    {
        _repository = repository;
        _loanRepository = loanRepository;
    }

    // Creates a book with a new id, not loaned, and adds it to the library.
    public Book AddBook(string title, string author)
    {
        var book = new Book
        {
            Id = Guid.NewGuid(),
            Title = title,
            Author = author
        };
        _repository.Add(book);
        return book;
    }

    // Returns all books. The list is read-only, so books can only be added or removed through this service.
    public IReadOnlyList<Book> GetBooks()
    {
        return _repository.GetAll();
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
        _repository.Update(book);
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
        _repository.Update(book);
    }

    // Removes the book from the library. Throws InvalidOperationException if no book has the id.
    public void DeleteBook(Guid bookId)
    {
        var book = GetBookById(bookId);
        if (book.IsLoaned)
        {
            throw new NotImplementedException();
        }
        _repository.Delete(book);
    }

    private Book GetBookById(Guid bookId)
    {
        return _repository.GetAll().First(b => b.Id == bookId);
    }

}
