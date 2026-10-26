using BookKeeper.Core;
using BookKeeper.Core.Repository;

namespace BookKeeper.Infrastructure;

// Stores the books in PostgreSQL through EF Core. Used by the API, while the tests use FakeBookRepository.
// Every method that changes data calls SaveChanges, so the change is written to the database right away.
public class EfBookRepository : IBookRepository
{
    private readonly BookKeeperDbContext _db;

    public EfBookRepository(BookKeeperDbContext db)
    {
        _db = db;
    }

    public void Add(Book book)
    {
        _db.Books.Add(book);
        _db.SaveChanges();
    }

    public IReadOnlyList<Book> GetAll() => _db.Books.ToList();

    public void Update(Book book)
    {
        _db.Books.Update(book);
        _db.SaveChanges();
    }

    public void Delete(Book book)
    {
        _db.Books.Remove(book);
        _db.SaveChanges();
    }

    public IReadOnlyList<Book> GetLoanedBy(Guid userId) => _db.Books.Where(b => b.LoanedByUserId == userId).ToList();
}
