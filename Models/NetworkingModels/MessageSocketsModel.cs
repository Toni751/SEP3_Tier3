using System;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class MessageSocketsModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("senderId")]
        public int SenderId { get; set; }
        
        [JsonPropertyName("receiverId")]
        public int ReceiverId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set;}
        
        [JsonPropertyName("hasImage")]
        public bool HasImage { get; set; }
    }
}