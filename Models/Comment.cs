using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class Comment
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("owner")]
        public User Owner { get; set; }
        
        [Required]
        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }
    }
}