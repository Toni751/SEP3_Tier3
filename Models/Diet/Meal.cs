using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a meal
    /// </summary>
    public class Meal
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [Required]
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [Required]
        [JsonPropertyName("calories")]
        public int Calories { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}