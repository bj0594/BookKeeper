using BookKeeper.Core;
using BookKeeper.Core.Repository;

namespace BookKeeper.Tests.Domain;

public class BookServiceTests
{
    private readonly BookService _service = new(new FakeBookRepository());

    [Fact]
    public void AddBook_ValidBook_AddsBookToLibrary()
    {
        // Act
        _service.AddBook("The Great Gatsby", "F. Scott Fitzgerald");

        // Assert
        Assert.Single(_service.GetBooks());
    }

    [Fact]
    public void LoanBook_ExistingBook_BecomesLoaned()
    {
        // Arrange
        var book = _service.AddBook("1984", "George Orwell");

        // Act
        _service.LoanBook(book.Id);

        // Assert
        var stored = _service.GetBooks().Single(b => b.Id == book.Id);
        Assert.True(stored.IsLoaned);
    }

    [Fact]
    public void LoanBook_AlreadyLoaned_ThrowsException()
    {
        // Arrange
        var book = _service.AddBook("To Kill a Mockingbird", "Harper Lee");
        _service.LoanBook(book.Id);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _service.LoanBook(book.Id));
    }

    [Fact]
    public void ReturnBook_ExistingLoanedBook_BecomesNotLoaned()
    {
        // Arrange
        var book = _service.AddBook("Pride and Prejudice", "Jane Austen");
        _service.LoanBook(book.Id);

        // Act
        _service.ReturnBook(book.Id);

        // Assert
        var stored = _service.GetBooks().Single(b => b.Id == book.Id);
        Assert.False(stored.IsLoaned);
    }

    [Fact]
    public void ReturnBook_NotLoanedBook_ThrowsException()
    {
        // Arrange
        var book = _service.AddBook("Moby Dick", "Herman Melville");

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _service.ReturnBook(book.Id));
    }

    [Fact]
    public void DeleteBook_ExistingBook_RemovesBookFromLibrary()
    {
        // Arrange
        var book = _service.AddBook("The Catcher in the Rye", "J.D. Salinger");

        // Act
        _service.DeleteBook(book.Id);

        // Assert
        Assert.Empty(_service.GetBooks());
    }
}
