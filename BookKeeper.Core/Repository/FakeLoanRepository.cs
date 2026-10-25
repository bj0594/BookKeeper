namespace BookKeeper.Core.Repository;

public class FakeLoanRepository : ILoanRepository
{
    private readonly List<Loan> _loans = new();

    public void Add(Loan loan) => _loans.Add(loan);
    public IReadOnlyList<Loan> GetAll() => _loans;
}
