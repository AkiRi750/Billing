using Billing.Models;

namespace Billing.Data
{
    public interface ICoinTokenTransactionsService
    {
        List<CoinTokenTransaction> Get();
        IEnumerable<CoinTokenTransaction> Get(long coinId);
        void Add(CoinTokenTransaction coinTokenTransactions);
        long GetLongestCoinTokenId();
    }
}
