using System;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing the sockets version of the user action model
    /// </summary>
    public class ModelActionSockets
    {
        [JsonPropertyName("senderId")]
        public int SenderId { get; set; }
        
        [JsonPropertyName("senderName")]
        public string SenderName { get; set; }
        
        [JsonPropertyName("receiverId")]
        public int ReceiverId { get; set; }
        
        [JsonPropertyName("actionType")]
        public string ActionType { get; set; }
        
        [JsonPropertyName("value")]
        public Object Value { get; set; }
    }
}