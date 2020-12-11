using System;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a private message between 2 users
    /// </summary>
    public class Message
    {
        public int Id { get; set; }
        
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
        
        public string Content { get; set; }
        public DateTime TimeStamp { get; set;}
        public bool HasImage { get; set; }
    }
}