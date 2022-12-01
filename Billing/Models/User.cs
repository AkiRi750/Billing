namespace Billing.Models
{
    public class User
    {
        public int Id { get; set; }
        public UserProfile UserProfile { get; set; }
        public int Rating { get; set; }
    }
}
