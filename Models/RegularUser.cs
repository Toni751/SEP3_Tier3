using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SEP3_T3.Models
{
    public class RegularUser
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [JsonPropertyName("password")]
        public string Password { get; set; }
        
        [JsonPropertyName("accountType")]
        public string AccountType { get; set; }
        
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("posts")]
        public List<int> PostIds { get; set; } 
        
        [JsonPropertyName("likedPosts")]
        public List<int> LikedPostIds { get; set;}
        
        [JsonPropertyName("avatar")]
        public byte[] Avatar { get; set; }
        
        [JsonPropertyName("profileBackground")]
        public byte[] ProfileBackground { get; set; }
        
        [JsonPropertyName("city")]
        public string City { get; set; }
    }
}