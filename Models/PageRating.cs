using System.ComponentModel.DataAnnotations;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a page rating
    /// </summary>
    public class PageRating
    {
        public int PageId { get; set; }
        public User Page { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        
        [Required]
        public int Rating { get; set; }
    }
}