using Billing.Models;

namespace Billing.Data
{
    public class UserService : IUserService
    {
        private readonly List<User> _users;

        public UserService()
        {
            _users = new List<User>()
            {
                new User() { Id = 1, UserProfile = new UserProfile() { Name = "boris", Amount = 0 }, Rating = 5000 },
                new User() { Id = 2, UserProfile = new UserProfile() { Name = "maria", Amount = 0 }, Rating = 1000 },
                new User() { Id = 3, UserProfile = new UserProfile() { Name = "oleg", Amount = 0 }, Rating = 800 }
            };
        }

        public List<User> Get() => _users;
    }
}
