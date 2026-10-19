namespace BookKeeper.Core.Repository;

// Where BookService stores its books. The service only knows this interface, so the storage can be replaced without changing the service.
public interface IBookRepository
{
    void Add(Book book);
    IReadOnlyList<Book> GetAll();
    void Delete(Book book);
}
