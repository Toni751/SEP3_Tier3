using System.Text.Json.Serialization;

namespace SEP3_T3.Models
{
    public class Comment
    {
        [JsonPropertyName("owner")]
        public UserShortVersion Owner { get; set; }
        
        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        [JsonPropertyName("timeStamp")]
        public string TimeStamp { get; set; }
    }
}