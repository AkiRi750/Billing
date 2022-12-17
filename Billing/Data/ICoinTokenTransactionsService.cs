using Billing.Models;

namespace Billing.Data
{
    public interface ICoinTokenTransactionsService
    {
        void Add(CoinTokenTransactions coinTokenTransactions);
    }
}
