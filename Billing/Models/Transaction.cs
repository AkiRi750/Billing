namespace Billing.Models
{
    public class Transaction
    {
        public long Id { get; set; }
        public long SenderId { get; set; }
        public long ReceiverId { get; set; }
    }
}
