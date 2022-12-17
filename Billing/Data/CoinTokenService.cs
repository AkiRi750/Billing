using Billing.Models;
using Billing.LinqExtensions;

namespace Billing.Data
{
    public class CoinTokenService : ICoinTokenService
    {
        private readonly List<CoinToken> _coinTokens = new List<CoinToken>();

        public List<CoinToken> Get() => _coinTokens;

        public void Add(CoinToken coin) => _coinTokens.Add(coin);

        /// <summary>
        /// Возвращает id монеты для её создания
        /// </summary>
        public long GetCoinEmissionId()
        {
            if (_coinTokens.Count == 0)
                return 1;
            return _coinTokens.Last().Id + 1;
        }

        public IEnumerable<long> ChangeCoinTokensOwner(User oldOwner, User newOwner, long amount)
        {
            foreach (var coinToken in _coinTokens.Where(c => c.OwnerId == oldOwner.Id).TakeLong(amount))
            {
                coinToken.OwnerId = newOwner.Id;
                yield return coinToken.Id;
            }
        }
    }
}
