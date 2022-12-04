using Billing.Models;

namespace Billing.Data
{
    public class CoinTokenService : ICoinTokenService
    {
        private readonly List<CoinToken> _coinTokens = new List<CoinToken>();

        public List<CoinToken> Get() => _coinTokens;

        public void Add(CoinToken coin) => _coinTokens.Add(coin);
    }
}
