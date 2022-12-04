using Billing.Models;

namespace Billing.Data
{
    public interface ICoinTokenService
    {
        List<CoinToken> Get();
        void Add(CoinToken coinToken);
    }
}
