namespace BookKeeper.Core;

// A user who can log in.
public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; } = "";
    public string PasswordHash { get; set; } = "";
}
