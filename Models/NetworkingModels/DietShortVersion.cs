using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class DietShortVersion
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("global")]
        public bool Global { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
    }
}