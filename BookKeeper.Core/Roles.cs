namespace BookKeeper.Core;

// The role names in the API. Used in AuthService, User and [Authorize(Roles = ...)].
public static class Roles
{
    public const string Librarian = "Librarian";
    public const string LibraryUser = "LibraryUser";
}
