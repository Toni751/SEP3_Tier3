using System;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class UserActionSockets
    {
        [JsonPropertyName("senderId")]
        public int SenderId { get; set; }
        
        [JsonPropertyName("receiverId")]
        public int ReceiverId { get; set; }
        
        [JsonPropertyName("actionType")]
        public string ActionType { get; set; }
        
        [JsonPropertyName("value")]
        public Object Value { get; set; }
    }
}