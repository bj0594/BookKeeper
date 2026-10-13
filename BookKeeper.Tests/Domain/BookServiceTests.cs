using BookKeeper.Core;
namespace BookKeeper.Tests;

public class BookServiceTests
{
    private readonly BookService _service = new();

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
        var stored = _service.GetBooks().Single(i => i.Id == book.Id);
        Assert.True(stored.IsLoaned);
    }
}
