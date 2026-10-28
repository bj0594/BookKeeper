namespace BookKeeper.Core.Repository;

// Where AuthService stores the users. EfUserRepository in the API, FakeUserRepository in the tests.
public interface IUserRepository
{
    void Add(User user);
    User? GetByUsername(string username);
    bool HasRole(string role);
}
