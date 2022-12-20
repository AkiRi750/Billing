using Billing.Data;
using Billing.Models;
using Grpc.Core;

namespace Billing
{
    public class BillingService : Billing.BillingBase
    {
        private readonly IUserService _userService;
        private readonly ICoinTokenService _coinTokenService;
        private readonly ITransactionService _transactionService;
        private readonly ICoinTokenTransactionsService _coinTokenTransactionsService;
        private readonly object lockObject = new object();

        public BillingService
            (IUserService userService, 
            ICoinTokenService coinTokenService,
            ITransactionService transactionService,
            ICoinTokenTransactionsService coinTokenTransactionsService)
        {
            _userService = userService;
            _coinTokenService = coinTokenService;
            _transactionService = transactionService;
            _coinTokenTransactionsService = coinTokenTransactionsService;
        }

        public override async Task ListUsers
            (None request,
            IServerStreamWriter<UserProfile> responseStream,
            ServerCallContext context)
        {
            var users = _userService.Get().Select(user => user.UserProfile);
            foreach (var user in users)
                await responseStream.WriteAsync(user);
        }

        public override Task<Response> CoinsEmission(EmissionAmount request, ServerCallContext context)
        {
            var task = Task.Run(() =>
            {
                if (request.Amount <= 0 || request.Amount < _userService.Get().Count)
                    return new Response()
                    {
                        Status = Response.Types.Status.Failed,
                        Comment = "Invalid coin emission ammount"
                    };

                var users = _userService.Get();
                var totalRating = users.Sum(x => x.Rating);
                var averageRating = totalRating / users.Count;
                if (request.Amount < GetMinEmissionAmount(users, averageRating))
                    return new Response()
                    {
                        Status = Response.Types.Status.Failed,
                        Comment = "Not enough coins to emission"
                    };

                return CoinsEmissionOperation(users, request, totalRating);
            });

            return task;
        }

        /// <summary>
        /// Операция эмиссии монет
        /// </summary>
        private Response CoinsEmissionOperation(List<User> users, EmissionAmount request, long totalRating)
        {
            long undistributedCoins = request.Amount;
            User userWithMaxRating = users.First();
            foreach (var user in users)
            {
                var emissionAmount = GetEmissionAmountPerUser(user, totalRating, request.Amount);
                EmissionCoinsToUser(user, emissionAmount);
                if (user.Rating > userWithMaxRating.Rating)
                    userWithMaxRating = user;
                undistributedCoins -= emissionAmount;
            }

            // Компенсация погрешности распределения GetEmissionAmountPerUser()
            if (undistributedCoins > 0)
                EmissionCoinsToUser(userWithMaxRating, undistributedCoins);

            return new Response() { Status = Response.Types.Status.Ok };
        }

        /// <summary>
        /// Вычисляет какое минимальное количество монет можно выпустить для пользователей, 
        /// с учётом пропорциональности распределения
        /// </summary>
        private long GetMinEmissionAmount(List<User> users, long averageRating)
            => users.Select(user => Math.Max(user.Rating / averageRating, 1L)).Sum();

        /// <summary>
        /// Вычисляет количество монет, зачисляемых пользователю при выпуске монет
        /// </summary>
        private long GetEmissionAmountPerUser(User user, long totalRating, long totalAmount = 1)
            => Math.Max(user.Rating * totalAmount / totalRating, 1L);

        /// <summary>
        /// Выпускает данное количество монет для пользователя
        /// </summary>
        private void EmissionCoinsToUser(User user, long amount)
        {
            for (long i = 0; i < amount; i++)
            {
                var coinToken = new CoinToken() 
                { 
                    Id = _coinTokenService.GetCoinEmissionId(), 
                    OwnerId = user.Id 
                };

                lock (lockObject)
                {
                    user.UserProfile.Amount++;
                    _coinTokenService.Add(coinToken);
                }
            }
        }

        public override Task<Response> MoveCoins(MoveCoinsTransaction request, ServerCallContext context)
        {
            var task = Task.Run(() =>
            {
                var response = IsMovingCoinsPossible(request);
                if (response.Status == Response.Types.Status.Failed)
                    return response;

                var sourceUser = _userService.GetUser(request.SrcUser);
                var destinationUser = _userService.GetUser(request.DstUser);
                lock(lockObject)
                {
                    MovingCoins(sourceUser, destinationUser, request.Amount);
                }

                return new Response() { Status = Response.Types.Status.Ok };
            });

            return task;
        }

        /// <summary>
        /// Операция передачи монет
        /// </summary>
        private void MovingCoins(User sourceUser, User destinationUser, long amount)
        {
            _userService.AmountTransfer(sourceUser, destinationUser, amount);
            var transactionId = _transactionService.GetTransactionCreationId();
            _transactionService.Add(new Transaction()
            {
                Id = transactionId,
                SenderId = sourceUser.Id,
                ReceiverId = destinationUser.Id
            });
            var changedCoinIds = _coinTokenService.ChangeCoinTokensOwner(sourceUser, destinationUser, amount);
            
            foreach (var coinToken in changedCoinIds)
            {
                _coinTokenTransactionsService.Add(new CoinTokenTransactions()
                {
                    TransactionId = transactionId,
                    CoinTokenId = coinToken
                });
            }
        }

        /// <summary>
        /// Проверка возможности перевода монет между пользователями
        /// </summary>
        private Response IsMovingCoinsPossible(MoveCoinsTransaction request)
        {
            if (request.Amount <= 0)
                return new Response()
                {
                    Status = Response.Types.Status.Failed,
                    Comment = $"Invalid coins movement amount"
                };

            if (request.SrcUser == request.DstUser)
                return new Response()
                {
                    Status = Response.Types.Status.Failed,
                    Comment = $"Coins cannot be moved to yourself"
                };

            if (!_userService.IsUserExists(request.SrcUser))
                return new Response()
                {
                    Status = Response.Types.Status.Failed,
                    Comment = $"User {request.SrcUser} is not exists"
                };

            if (!_userService.IsUserExists(request.DstUser))
                return new Response()
                {
                    Status = Response.Types.Status.Failed,
                    Comment = $"User {request.DstUser} is not exists"
                };

            var sourceUser = _userService.GetUser(request.SrcUser);
            if (sourceUser.UserProfile.Amount < request.Amount)
                return new Response()
                {
                    Status = Response.Types.Status.Failed,
                    Comment = $"User {request.SrcUser} doesn't have enough coins to transfer"
                };

            return new Response() { Status = Response.Types.Status.Ok };
        }

        public override Task<Coin> LongestHistoryCoin(None request, ServerCallContext context)
        {
            return base.LongestHistoryCoin(request, context);
        }
    }
}