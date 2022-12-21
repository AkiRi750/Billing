using Billing.Models;

namespace Billing.Data
{
    public interface IUserService
    {
        List<User> Get();
        User GetUser(string username);
        User GetUser(long id);
        bool IsUserExists(string user);
        void AmountTransfer(User sourceUser, User destinationUser, long amount);
    }
}
