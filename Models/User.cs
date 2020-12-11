using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing both regular users and gyms
    /// </summary>
    public class User
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        [Required]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        
        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        //you access a user's (or page's) posts only when you go to his/her profile (same applies if it is your own)
        //so it would make more sense to have a list of posts I think 
        //ALSO DON'T FORGET ABOUT LIMIT
        [JsonIgnore]
        public List<Post> Posts { get; set; }

        // [JsonPropertyName("avatar")]
        // public byte[] Avatar { get; set; }
        //
        // [JsonPropertyName("profileBackground")]
        // public byte[] ProfileBackground { get; set; }
        
        [Required]
        [JsonPropertyName("city")]
        public string City { get; set; }
        
        [DefaultValue(0)]
        [JsonPropertyName("score")]
        public int Score { get; set; }
        
        [JsonPropertyName("address")]
        public Address Address { get; set; }
    }
}