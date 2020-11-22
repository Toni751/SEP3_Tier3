using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class User
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
        
        //you access a user's (or page's) posts only when you go to his/her profile (same applies if it is your own)
        //so it would make more sense to have a list of posts I think 
        //ALSO DON'T FORGET ABOUT LIMIT
        [JsonPropertyName("posts")]
        public List<Post> Posts { get; set; }
        
        public List<Exercise> Exercises { get; set; }
        public List<Training> Trainings { get; set; }
        
        public List<Meal> Meals { get; set; }
        public List<Diet> Diets { get; set; }

        // [JsonPropertyName("avatar")]
        // public byte[] Avatar { get; set; }
        //
        // [JsonPropertyName("profileBackground")]
        // public byte[] ProfileBackground { get; set; }
        
        [JsonPropertyName("city")]
        public string City { get; set; }
        
        [DefaultValue(0)]
        [JsonPropertyName("score")]
        public int Score { get; set; }
        
        [JsonPropertyName("address")]
        public Address Address { get; set; }
    }
}