using Billing.Models;

namespace Billing.Data
{
    public class TransactionService : ITransactionService
    {
        private readonly List<Transaction> _transactions = new List<Transaction>();

        public void Add(Transaction transaction) => _transactions.Add(transaction);

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
