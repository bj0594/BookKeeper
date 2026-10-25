namespace BookKeeper.Core;

public class Loan
{
    public Guid Id { get; set; }
    public Guid BookId { get; set; }
    public Guid UserId { get; set; }
}
