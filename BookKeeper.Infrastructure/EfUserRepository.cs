using BookKeeper.Core;
using BookKeeper.Core.Repository;

namespace BookKeeper.Infrastructure;

public class EfUserRepository : IUserRepository
{
    private readonly BookKeeperDbContext _db;

    public EfUserRepository(BookKeeperDbContext db)
    {
        _db = db;
    }

    public void Add(User user)
    {
        _db.Users.Add(user);
        _db.SaveChanges();
    }

    public User? GetByUsername(string username) => _db.Users.FirstOrDefault(u => u.Username == username);
}
