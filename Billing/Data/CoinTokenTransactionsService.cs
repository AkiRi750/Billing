using Billing.LinqExtensions;
using Billing.Models;
using System.Collections.Generic;

namespace Billing.Data
{
    public class CoinTokenTransactionsService : ICoinTokenTransactionsService
    {
        private readonly List<CoinTokenTransaction> _coinTokenTransactions = 
            new List<CoinTokenTransaction>();

        public List<CoinTokenTransaction> Get() => _coinTokenTransactions;

        public IEnumerable<CoinTokenTransaction> Get(long coinId)
            => _coinTokenTransactions.Where(x => x.CoinTokenId == coinId);

        public void Add(CoinTokenTransaction coinTokenTransactions) 
            => _coinTokenTransactions.Add(coinTokenTransactions);

        public long GetLongestCoinTokenId()
            => _coinTokenTransactions.GroupBy(x => x.CoinTokenId)
                .Select(x => new { x.Key, Value = x.CountLong() })
                .MaxBy(x => x.Value)!
                .Value;
    }
}
