using Billing.Models;

namespace Billing.Data
{
    public class CoinTokenTransactionsService : ICoinTokenTransactionsService
    {
        private readonly List<CoinTokenTransactions> _coinTokenTransactions = 
            new List<CoinTokenTransactions>();

        public void Add(CoinTokenTransactions coinTokenTransactions) 
            => _coinTokenTransactions.Add(coinTokenTransactions);
    }
}
