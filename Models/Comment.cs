﻿using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class Comment
    {
        public int Id { get; set; }
        
        [JsonPropertyName("owner")]
        public User Owner { get; set; }
        
        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        [JsonPropertyName("timeStamp")]
        public string TimeStamp { get; set; }
    }
}