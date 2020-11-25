namespace SEP3_Tier3.Models
{
    public class Notification
    {
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
        public string NotificationType { get; set; }
    }
}