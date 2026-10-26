namespace BookKeeper.Core;

// A book in the library. IsLoaned is true while the book is loaned out.
public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = "";
    public string Author { get; set; } = "";
    public bool IsLoaned { get; set; }
    public Guid? LoanedByUserId { get; set; }
}
