using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class TrainingSocketsModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
        
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }
        
        [JsonPropertyName("duration")]
        public int Duration { get; set; }
        
        [JsonPropertyName("isCompleted")]
        public bool IsCompleted { get; set; }
        
        [JsonPropertyName("global")]
        public bool IsPublic{ get; set; }
        
        [JsonPropertyName("exercises")]
        public List<Exercise> Exercises { get; set; }
    }
}