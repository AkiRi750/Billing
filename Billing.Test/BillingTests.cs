using Billing.Data;
using Billing.Models;
using Grpc.Core;
using Moq;
using Xunit.Sdk;

namespace Billing.Test
{
    public class BillingTests
    {
        #region CoinEmissionTests
        [Theory]
        [InlineData(-42)]
        [InlineData(0)]
        public async void CoinsEmissionWhenInvalidAmount(int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            _userService.Setup(service => service.Get())
                .Returns(GetUsers());
            var billingService = new BillingService
                (_userService.Object,
                _coinTokenService.Object,
                null,
                null);
            var request = new EmissionAmount() { Amount = amount };

            var result = await billingService.CoinsEmission(request, null);

            Assert.Equal(Response.Types.Status.Failed, result.Status);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        public async void CoinsEmissionWhenUsersAreLessThanCoins(int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            _userService.Setup(service => service.Get())
                .Returns(GetUsers());
            var billingService = new BillingService
                (_userService.Object, 
                _coinTokenService.Object,
                null,
                null);
            var request = new EmissionAmount() { Amount = amount };

            var result = await billingService.CoinsEmission(request, null);

            Assert.Equal(Response.Types.Status.Failed, result.Status);
        }

        [Theory]
        [InlineData(42)]
        [InlineData(100)]
        public async void CoinsEmission(int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            _userService.Setup(service => service.Get())
                .Returns(GetUsers());
            _coinTokenService.Setup(service => service.Get())
                .Returns(new List<CoinToken>());
            var billingService = new BillingService
                (_userService.Object,
                _coinTokenService.Object,
                null,
                null);
            var request = new EmissionAmount() { Amount = amount };

            var result = await billingService.CoinsEmission(request, null);

            Assert.Equal(Response.Types.Status.Ok, result.Status);
        }

        /// <summary>
        /// Эмиссия невозможна, поскольку количество монет недостаточно, что бы можно было
        /// распределить их пропорционально среди пользователей с сильно разным рейтингом
        /// </summary>
        [Theory]
        [InlineData(3)]
        public async void CoinsEmissionWhenAmountIsNotEnough(int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            _userService.Setup(service => service.Get())
                .Returns(GetUsers());
            var billingService = new BillingService
                (_userService.Object,
                _coinTokenService.Object,
                null,
                null);
            var request = new EmissionAmount() { Amount = amount };

            var result = await billingService.CoinsEmission(request, null);

            Assert.Equal(Response.Types.Status.Failed, result.Status);
        }
        #endregion

        #region MoveCoinsTests
        [Theory]
        [InlineData("Dan", "Bob", 42)]
        public async void MoveCoinsWhenSourceUserIsNotExists(string source, string destination, int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            _userService.Setup(service => service.IsUserExists(source))
                .Returns(false);
            _userService.Setup(service => service.IsUserExists(destination))
                .Returns(true);
            var billingService = new BillingService
                (_userService.Object,
                _coinTokenService.Object,
                null,
                null);
            var request = new MoveCoinsTransaction() 
                { SrcUser = source, DstUser = destination, Amount = amount };

            var result = await billingService.MoveCoins(request, null);

            Assert.Equal(Response.Types.Status.Failed, result.Status);
        }

        [Theory]
        [InlineData("Bob", "Dan", 42)]
        public async void MoveCoinsWhenDestinationUserIsNotExists(string source, string destination, int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            _userService.Setup(service => service.IsUserExists(source))
                .Returns(true);
            _userService.Setup(service => service.IsUserExists(destination))
                .Returns(false);
            var billingService = new BillingService
                (_userService.Object,
                _coinTokenService.Object,
                null,
                null);
            var request = new MoveCoinsTransaction()
            { SrcUser = source, DstUser = destination, Amount = amount };

            var result = await billingService.MoveCoins(request, null);

            Assert.Equal(Response.Types.Status.Failed, result.Status);
        }

        [Theory]
        [InlineData("Bob", "Dan", 0)]
        [InlineData("Bob", "Dan", -1000)]
        public async void MoveCoinsWhenInvalidRequestAmount(string source, string destination, int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            _userService.Setup(service => service.IsUserExists(source))
                .Returns(true);
            _userService.Setup(service => service.IsUserExists(destination))
                .Returns(false);
            var billingService = new BillingService
                (_userService.Object,
                _coinTokenService.Object,
                null,
                null);
            var request = new MoveCoinsTransaction()
            { SrcUser = source, DstUser = destination, Amount = amount };

            var result = await billingService.MoveCoins(request, null);

            Assert.Equal(Response.Types.Status.Failed, result.Status);
        }

        [Theory]
        [InlineData("Bob", "Bob", 42)]
        public async void MoveCoinsWhenSourceUserIsDestinationUser(string source, string destination, int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            var billingService = new BillingService
                (_userService.Object,
                _coinTokenService.Object,
                null,
                null);
            var request = new MoveCoinsTransaction()
            { SrcUser = source, DstUser = destination, Amount = amount };

            var result = await billingService.MoveCoins(request, null);

            Assert.Equal(Response.Types.Status.Failed, result.Status);
        }

        [Theory]
        [InlineData("Tom", "Bob", 1000000)]
        public async void MoveCoinsWhenSourceUserHaveNotEnoughCoins(string source, string destination, int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            _userService.Setup(service => service.IsUserExists(source))
                .Returns(true);
            _userService.Setup(service => service.IsUserExists(destination))
                .Returns(true);
            _userService.Setup(service => service.GetUser(source))
                .Returns(GetUsers()[0]);
            var billingService = new BillingService
                (_userService.Object,
                _coinTokenService.Object,
                null,
                null);
            var request = new MoveCoinsTransaction()
            { SrcUser = source, DstUser = destination, Amount = amount };

            var result = await billingService.MoveCoins(request, null);

            Assert.Equal(Response.Types.Status.Failed, result.Status);
        }

        [Theory]
        [InlineData("Tom", "Bob", 10)]
        [InlineData("Tom", "Bob", 100)]
        public async void MoveCoins(string source, string destination, int amount)
        {
            var _userService = new Mock<IUserService>();
            var _coinTokenService = new Mock<ICoinTokenService>();
            var _transactionService = new Mock<ITransactionService>();
            var _coinTokenTransactionService = new Mock<ICoinTokenTransactionsService>();
            _userService.Setup(service => service.IsUserExists(source))
                .Returns(true);
            _userService.Setup(service => service.IsUserExists(destination))
                .Returns(true);
            _userService.Setup(service => service.GetUser(source))
                .Returns(GetUsers()[0]);
            _userService.Setup(service => service.GetUser(destination))
                .Returns(GetUsers()[2]);
            var billingService = new BillingService
                (_userService.Object,
                _coinTokenService.Object,
                _transactionService.Object,
                _coinTokenTransactionService.Object);
            var request = new MoveCoinsTransaction()
            { SrcUser = source, DstUser = destination, Amount = amount };

            var result = await billingService.MoveCoins(request, null);

            Assert.Equal(Response.Types.Status.Ok, result.Status);
        }
        #endregion

        private List<User> GetUsers() =>
            new List<User>()
            {
                new User() { Id = 1, Rating = -2000, UserProfile = new UserProfile() { Name = "Tom", Amount = 100 } },
                new User() { Id = 1, Rating = 500, UserProfile = new UserProfile() { Name = "Tim", Amount = 0 } },
                new User() { Id = 1, Rating = 4000, UserProfile = new UserProfile() { Name = "Bob", Amount = 0 } }
            };
    }
}