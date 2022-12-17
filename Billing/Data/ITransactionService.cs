using Billing.Models;

namespace Billing.Data
{
    public interface ITransactionService
    {
        long GetTransactionCreationId();
        void Add(Transaction transaction);
    }
}
