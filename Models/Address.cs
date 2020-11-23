using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class Address
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [Required]
        [JsonPropertyName("street")]
        public string Street { get; set; }
        
        [Required]
        [JsonPropertyName("number")]
        public string Number { get; set; }
        
    }
}