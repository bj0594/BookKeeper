namespace BookKeeper.Web.Api;

// One book as the API sends it. The fields are the ones in the response to POST /books in api.md.
public record BookDto(Guid Id, string Title, string Author, bool IsLoaned, Guid? LoanedByUserId);
