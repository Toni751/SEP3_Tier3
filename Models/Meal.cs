using System.ComponentModel.DataAnnotations;

namespace SEP3_Tier3.Models
{
    public class Meal
    {
        public int Id { get; set; }
        
        [Required]
        public string Title { get; set; }
        [Required]
        public int Calories { get; set; }
        public string Description { get; set; }
    }
}