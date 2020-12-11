using System.ComponentModel;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a user action (such as friend request, report, follow page etc)
    /// </summary>
    public class UserAction
    {
        public int SenderId { get; set; }
        public User Sender { get; set; }
        public int ReceiverId { get; set; }
        public User Receiver { get; set; }
        
        [DefaultValue(false)]
        public bool IsFriendRequest { get; set; }
        
        [DefaultValue(false)]
        public bool IsReport { get; set; }
        
        [DefaultValue(false)]
        public bool IsShareTrainings { get; set; }
        
        [DefaultValue(false)]
        public bool IsShareDiets { get; set; }
        
        [DefaultValue(false)]
        public bool IsFollowPage { get; set; }
    }
}