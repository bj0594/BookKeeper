namespace BookKeeper.Core.Repository;

// Where BookService stores its books. The service only knows this interface, so the storage can be replaced without changing the service.
public interface IBookRepository
{
    void Add(Book book);
    IReadOnlyList<Book> GetAll();
    Book? GetById(Guid id);
    IReadOnlyList<Book> GetAvailable();
    void Delete(Book book);
    void Update(Book book);
    IReadOnlyList<Book> GetLoanedBy(Guid userId);
}
