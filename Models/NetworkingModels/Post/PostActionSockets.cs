using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing the sockets version of the post action model
    /// </summary>
    public class PostActionSockets
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        
        [JsonPropertyName("postId")]
        public int PostId { get; set; }
        
        [JsonPropertyName("actionType")]
        public string ActionType { get; set; }
        
        [JsonPropertyName("value")]
        public bool Value { get; set; }
    }
}