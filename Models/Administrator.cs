using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class Administrator
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("password")]
        public string Password { get; set; }
    }
}