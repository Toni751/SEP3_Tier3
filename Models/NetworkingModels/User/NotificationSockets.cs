using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing the sockets version of the notification model
    /// </summary>
    public class NotificationSockets
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("senderId")]
        public int SenderId { get; set; }
        
        [JsonPropertyName("senderName")]
        public string SenderName { get; set; }
        
        [JsonPropertyName("receiverId")]
        public int ReceiverId { get; set; }
        
        [JsonPropertyName("actionType")]
        public string ActionType { get; set; }
    }
}