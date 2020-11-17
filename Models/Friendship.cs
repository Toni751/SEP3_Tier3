namespace SEP3_Tier3.Models
{
    public class Friendship
    {
        public int FirstUserId { get; set; }
        public User FirstUser { get; set; }
        public int SecondUserId { get; set; }
        public User SecondUser { get; set; }
    }
}