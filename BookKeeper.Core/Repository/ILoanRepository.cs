namespace BookKeeper.Core.Repository;

public interface ILoanRepository
{
    void Add(Loan loan);
    IReadOnlyList<Loan> GetAll();
}
