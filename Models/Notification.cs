﻿using System.ComponentModel.DataAnnotations;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a notification for a user
    /// </summary>
    public class Notification
    {
        public int Id { get; set; }
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
        [MaxLength(50)]
        public string NotificationType { get; set; }
    }
}