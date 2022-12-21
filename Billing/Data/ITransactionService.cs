using Billing.Models;

namespace Billing.Data
{
    public interface ITransactionService
    {
        long GetTransactionCreationId();
        Transaction Get(CoinTokenTransaction coinTokenTransaction);
        IEnumerable<Transaction> Get(IEnumerable<CoinTokenTransaction> coinTokenTransactions);
        void Add(Transaction transaction);
    }
}
