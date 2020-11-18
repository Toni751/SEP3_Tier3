using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class UserSocketsModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }

        [JsonPropertyName("city")]
        public string City { get; set; }
        
        [JsonPropertyName("score")]
        public int Score { get; set; }
        
        [JsonPropertyName("address")]
        public Address Address { get; set; }
        
        [JsonPropertyName("avatar")]
        public byte[] Avatar { get; set; }
        
        [JsonPropertyName("background")]
        public byte[] Background { get; set; }
        
        
    }
}