using BookKeeper.Core;
using BookKeeper.Core.Repository;

namespace BookKeeper.Tests.Domain;

public class BookServiceTests
{
    private readonly BookService _service = new(new FakeBookRepository());
    private readonly Guid _userId = Guid.NewGuid();
    private Book AddBook() => _service.AddBook("1984", "George Orwell");

    [Fact]
    public void AddBook_ValidBook_AddsBookToLibrary()
    {
        // Act
        AddBook();

        // Assert
        Assert.Single(_service.GetBooks());
    }

    [Fact]
    public void LoanBook_ExistingBook_BecomesLoaned()
    {
        // Arrange
        var book = AddBook();

        // Act
        _service.LoanBook(book.Id, _userId);

        // Assert
        var stored = _service.GetBooks().Single(b => b.Id == book.Id);
        Assert.True(stored.IsLoaned);
    }

    [Fact]
    public void LoanBook_AlreadyLoaned_ThrowsException()
    {
        // Arrange
        var book = AddBook();
        _service.LoanBook(book.Id, _userId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _service.LoanBook(book.Id, _userId));
    }

    [Fact]
    public void ReturnBook_ExistingLoanedBook_BecomesNotLoaned()
    {
        // Arrange
        var book = AddBook();
        _service.LoanBook(book.Id, _userId);

        // Act
        _service.ReturnBook(book.Id, _userId);

        // Assert
        var stored = _service.GetBooks().Single(b => b.Id == book.Id);
        Assert.False(stored.IsLoaned);
    }

    [Fact]
    public void ReturnBook_NotLoanedBook_ThrowsException()
    {
        // Arrange
        var book = AddBook();

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _service.ReturnBook(book.Id, _userId));
    }

    [Fact]
    public void DeleteBook_ExistingBook_RemovesBookFromLibrary()
    {
        // Arrange
        var book = AddBook();

        // Act
        _service.DeleteBook(book.Id);

        // Assert
        Assert.Empty(_service.GetBooks());
    }

    [Fact]
    public void DeleteBook_AlreadyLoaned_ThrowsInvalidOperationException()
    {
        // Arrange
        var book = AddBook();
        _service.LoanBook(book.Id, _userId);

        // Act & Assert
        Assert.Throws<InvalidOperationException>(() => _service.DeleteBook(book.Id));
    }

    [Fact]
    public void GetAvailableBooks_OneIsLoaned_ReturnsOnlyTheOther()
    {
        // Arrange
        var book = AddBook();
        var loanedBook = AddBook();
        _service.LoanBook(loanedBook.Id, _userId);

        // Act
        var result = _service.GetAvailableBooks();

        // Assert
        var found = Assert.Single(result);
        Assert.Equal(book.Id, found.Id);
    }

    [Fact]
    public void LoanBook_ExistingBook_SetsLoanedByUserId()
    {
        // Arrange
        var book = AddBook();
        var userId = Guid.NewGuid();

        // Act
        _service.LoanBook(book.Id, userId);

        // Assert
        Assert.Equal(userId, book.LoanedByUserId);
    }

    [Fact]
    public void GetLoans_TwoUsers_ReturnsOnlyOwn()
    {
        // Arrange
        var ownBook = AddBook();
        var otherBook = AddBook();
        _service.LoanBook(ownBook.Id, _userId);
        _service.LoanBook(otherBook.Id, Guid.NewGuid());

        // Act
        var result = _service.GetLoans(_userId);

        // Assert
        var found = Assert.Single(result);
        Assert.Equal(ownBook.Id, found.Id);
    }

    [Fact]
    public void ReturnBook_OtherUsersLoan_ThrowsUnauthorizedAccessException()
    {
        // Arrange
        var book = AddBook();
        _service.LoanBook(book.Id, Guid.NewGuid());

        // Act + Assert
        Assert.Throws<UnauthorizedAccessException>(() => _service.ReturnBook(book.Id, _userId));
    }

    [Fact]
    public void ReturnBook_LoanedBook_RemovesItFromGetLoans()
    {
        // Arrange
        var book = AddBook();
        _service.LoanBook(book.Id, _userId);

        // Act
        _service.ReturnBook(book.Id, _userId);

        // Assert
        Assert.Empty(_service.GetLoans(_userId));

    }
}
