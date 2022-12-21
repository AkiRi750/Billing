using Billing.Models;

namespace Billing.Data
{
    public class TransactionService : ITransactionService
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public void Add(Transaction transaction) => _transactions.Add(transaction);

        public Transaction Get(CoinTokenTransaction coinTokenTransaction)
            => _transactions.Single(x => x.Id == coinTokenTransaction.TransactionId);

        public IEnumerable<Transaction> Get(IEnumerable<CoinTokenTransaction> coinTokenTransactions)
            => coinTokenTransactions.Select(x => Get(x));

        /// <summary>
        /// Возвращает id транзакции для её создания
        /// </summary>
        public long GetTransactionCreationId()
        {
            if (_transactions.Count == 0)
                return 1;
            return _transactions.Last().Id + 1;
        }
    }
}
