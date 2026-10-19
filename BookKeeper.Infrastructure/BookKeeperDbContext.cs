using Microsoft.EntityFrameworkCore;
using BookKeeper.Core;

namespace BookKeeper.Infrastructure;

public class BookKeeperDbContext : DbContext
{
    public BookKeeperDbContext(DbContextOptions<BookKeeperDbContext> options) : base(options) {}

    public DbSet<Book> Books => Set<Book>();
}
