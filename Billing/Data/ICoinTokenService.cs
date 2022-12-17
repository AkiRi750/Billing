using Billing.Models;

namespace Billing.Data
{
    public interface ICoinTokenService
    {
        List<CoinToken> Get();
        long GetCoinEmissionId();
        void Add(CoinToken coinToken);
        IEnumerable<long> ChangeCoinTokensOwner(User oldOwner, User newOwner, long amount);
    }
}
