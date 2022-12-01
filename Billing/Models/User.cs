namespace Billing.Models
{
    public class User
    {
        public long Id { get; set; }
        public UserProfile UserProfile { get; set; }
        public long Rating { get; set; }
    }
}
