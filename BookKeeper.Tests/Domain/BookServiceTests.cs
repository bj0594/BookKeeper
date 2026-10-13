using BookKeeper.Core;
namespace BookKeeper.Tests;

public class BookServiceTests
{
    [Fact]
    public void AddBook_ValidBook_AddsBookToLibrary()
    {
        // Arrange
        var service = new BookService();

        // Act
        service.AddBook("The Great Gatsby", "F. Scott Fitzgerald");

        // Assert
        Assert.Single(service.GetBooks());
    }

    [Fact]
    public void LoanBook_ExistingBook_BecomesLoaned()
    {
        // Arrange
        var service = new BookService();
        var book = service.AddBook("1984", "George Orwell");

        // Act
        service.LoanBook(book.Id);

        // Assert
        var stored = service.GetBooks().Single(i => i.Id == book.Id);
        Assert.True(stored.IsLoaned);
    }
}
