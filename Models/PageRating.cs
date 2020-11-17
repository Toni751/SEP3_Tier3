namespace SEP3_Tier3.Models
{
    public class PageRating
    {
        public int PageId { get; set; }
        public User Page { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        
        public int Rating { get; set; }
    }
}