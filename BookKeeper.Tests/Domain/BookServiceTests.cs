using BookKeeper.Core;
namespace BookKeeper.Tests;

public class BookServiceTests
{
    [Fact]
    public void AddBook_ValidBook_AddsBookToLibrary()
    {
        //Arrange
        var service = new BookService();

        //Act
        service.AddBook("The Great Gatsby", "F. Scott Fitzgerald");

        //Assert
        Assert.Single(service.GetBooks());
    }
}
