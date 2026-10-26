namespace BookKeeper.Core;

// A book in the library. It's loaned out when LoanedByUserId has a value.
public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public bool IsLoaned => LoanedByUserId.HasValue;
    public Guid? LoanedByUserId { get; set; }
}
