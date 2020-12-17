namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a friendship association between 2 users
    /// </summary>
    public class Friendship
    {
        public int FirstUserId { get; set; }
        public User FirstUser { get; set; }
        public int SecondUserId { get; set; }
        public User SecondUser { get; set; }
    }
}