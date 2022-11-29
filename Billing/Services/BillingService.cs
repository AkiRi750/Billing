using Billing.Data;
using Billing.Models;
using Grpc.Core;

namespace Billing
{
    public class BillingService : Billing.BillingBase
    {
        private readonly IUserRepository _userRepository;

        public BillingService(IUserRepository users)
        {
            _userRepository = users;
        }

        public override Task ListUsers
            (None request, 
            IServerStreamWriter<UserProfile> responseStream, 
            ServerCallContext context)
            => Task.FromResult(_userRepository.GetUsers().Select(user => user.UserProfile));

        public override Task<Response> CoinsEmission(EmissionAmount request, ServerCallContext context)
        {
            return base.CoinsEmission(request, context);
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
