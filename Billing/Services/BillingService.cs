using Billing.Data;
using Billing.Models;
using Grpc.Core;

namespace Billing
{
    public class BillingService : Billing.BillingBase
    {
        private readonly IUserService _userService;
        private readonly ICoinTokenService _coinTokenService;
        private readonly object lockObject = new object();

        public BillingService
            (IUserService userService, 
            ICoinTokenService coinTokenService)
        {
            _userService = userService;
            _coinTokenService = coinTokenService;
        }

        public override Task ListUsers
            (None request, 
            IServerStreamWriter<UserProfile> responseStream, 
            ServerCallContext context)
            => Task.FromResult(_userService.Get().Select(user => user.UserProfile));

        public override Task<Response> CoinsEmission(EmissionAmount request, ServerCallContext context)
        {
            if (request.Amount <= 0 || request.Amount < _userService.Get().Count)
                return Task.FromResult(new Response() 
                {
                    Status = Response.Types.Status.Failed, 
                    Comment = "Invalid coin emission ammount"
                });

            var task = Task.Run(() =>
            {
                var users = _userService.Get();
                var averageRating = users.Sum(x => x.Rating) / users.Count;
                if (request.Amount < GetMinEmissionAmount(users, averageRating))
                    return new Response()
                    {
                        Status = Response.Types.Status.Failed,
                        Comment = "Not enough coins to emission"
                    };

                foreach (var user in users)
                {
                    var emissionAmount = GetEmissionAmountPerUser(user, averageRating);
                    EmissionCoinsToUser(user, emissionAmount);
                }

                return new Response() { Status = Response.Types.Status.Ok };
            });

            return task;
        }

        private long GetMinEmissionAmount(List<User> users, long averageRating)
            => users.Select(user => GetEmissionAmountPerUser(user, averageRating)).Sum();

        private long GetEmissionAmountPerUser(User user, long averageRating)
            => Math.Max(user.Rating / averageRating, 1L);

        private void EmissionCoinsToUser(User user, long amount)
        {
            for (long i = 0; i < amount; i++)
            {
                var coinToken = new CoinToken() { Id = GetCoinEmissionId(), OwnerId = user.Id };
                lock (lockObject)
                {
                    _coinTokenService.Add(coinToken);
                }
            }
        }

        private long GetCoinEmissionId()
        {
            if (_coinTokenService.Get().Count == 0)
                return 1;
            return _coinTokenService.Get().Last().Id + 1;
        }

        public override Task<Response> MoveCoins(MoveCoinsTransaction request, ServerCallContext context)
        {
            return base.MoveCoins(request, context);
        }

        public override Task<Coin> LongestHistoryCoin(None request, ServerCallContext context)
        {
            return base.LongestHistoryCoin(request, context);
        }
    }
}
