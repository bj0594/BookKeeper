namespace BookKeeper.Api;

// The JSON body for POST /books.
public record CreateBookRequest(string Title, string Author);
