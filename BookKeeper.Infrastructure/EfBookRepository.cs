using BookKeeper.Core;
using BookKeeper.Core.Repository;

namespace BookKeeper.Infrastructure;

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
}
