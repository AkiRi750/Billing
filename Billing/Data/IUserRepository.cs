using Billing.Models;

namespace Billing.Data
{
    public interface IUserRepository
    {
        List<User> GetUsers();
    }
}
