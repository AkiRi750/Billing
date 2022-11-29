using Billing.Models;

namespace Billing.Data
{
    public class UserRepository : IUserRepository
    {
        private readonly List<User> _users;

        public UserRepository()
        {
            _users = new List<User>()
            {
                new User() { UserProfile = new UserProfile() { Name = "boris", Amount = 0 }, Rating = 5000 },
                new User() { UserProfile = new UserProfile() { Name = "maria", Amount = 0 }, Rating = 1000 },
                new User() { UserProfile = new UserProfile() { Name = "oleg", Amount = 0 }, Rating = 800 }
            };
        }

        public List<User> GetUsers() => _users;
    }
}
