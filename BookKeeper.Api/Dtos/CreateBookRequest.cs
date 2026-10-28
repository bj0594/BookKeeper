namespace BookKeeper.Api.Dtos;

// The JSON body for POST /books.
public record CreateBookRequest(string Title, string Author);
