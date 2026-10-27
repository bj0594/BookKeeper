using Microsoft.EntityFrameworkCore;
using BookKeeper.Core;

namespace BookKeeper.Infrastructure;

// The EF Core connection to PostgreSQL. Each DbSet is a table, so Books becomes the table "Books".
public class BookKeeperDbContext : DbContext
{
    public BookKeeperDbContext(DbContextOptions<BookKeeperDbContext> options) : base(options) {}

    public DbSet<Book> Books => Set<Book>();

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
    }
}
