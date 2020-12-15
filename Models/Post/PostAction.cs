using System.ComponentModel;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a post action (such as like or report)
    /// </summary>
    public class PostAction
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int PostId { get; set; }
        public Post Post { get; set; }
        
        [DefaultValue(false)]
        public bool IsLike { get; set; }
        [DefaultValue(false)]
        public bool IsReport { get; set; }
    }
}