namespace BookKeeper.Core.Repository;

// Keeps the users in memory. Used by the API tests instead of the database.
public class FakeUserRepository : IUserRepository
{
    private readonly List<User> _users = [];

    public void Add(User user) => _users.Add(user);

    public User? GetByUsername(string username) => _users.FirstOrDefault(u => u.Username == username);

    public bool HasRole(string role) => _users.Any(u => u.Role == role);
}
