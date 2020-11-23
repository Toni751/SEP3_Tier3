using System.ComponentModel.DataAnnotations;

namespace SEP3_Tier3.Models
{
    public class Exercise
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public string Type { get; set; }
        public string Description { get; set; }
        
    }
}