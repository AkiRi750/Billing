using Billing.Data;
using Billing.Models;
using Grpc.Core;
using Moq;
using Xunit.Sdk;

namespace Billing.Test
{
    public class BillingTests
    {
        [Theory]
        [InlineData(-42)]
        [InlineData(0)]
        public async void InvalidCoinAmountEmission(int amount)
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
        public async void EmissionWhenUsersAreLessThanCoins(int amount)
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
        public async void EmissionToUsers(int amount)
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
        public async void EmissionWhenAmountIsNotEnough(int amount)
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

        private List<User> GetUsers() =>
            new List<User>()
            {
                new User() { Id = 1, Rating = -2000, UserProfile = new UserProfile() { Name = "Tom"} },
                new User() { Id = 1, Rating = 500, UserProfile = new UserProfile() { Name = "Tim"} },
                new User() { Id = 1, Rating = 4000, UserProfile = new UserProfile() { Name = "Bob"} }
            };
    }
}