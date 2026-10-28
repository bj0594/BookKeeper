namespace BookKeeper.Web.Api;

// Holds the logged-in user's token and role for this browser tab. Reloading the page empties it.
public class TokenStore
{
    public string? Token { get; set; }
    public string? Role { get; set; }

    public bool IsLoggedIn => Token is not null;

    public void LogOut()
    {
        Token = null;
        Role = null;
    }
}
