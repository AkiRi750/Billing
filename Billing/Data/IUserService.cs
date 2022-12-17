using Billing.Models;

namespace Billing.Data
{
    public interface IUserService
    {
        List<User> Get();
        User GetUser(string username);
        bool IsUserExists(string user);
        bool IsEnoughCoinsToTransfer(User sourceUser, long amount);
        void AmountTransfer(User sourceUser, User destinationUser, long amount);
    }
}
