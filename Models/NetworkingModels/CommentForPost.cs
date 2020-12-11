using System;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a comment for a post
    /// </summary>
    public class CommentForPost
    {
        public int Id { get; set; }
        
        [JsonPropertyName("ownerId")]
        public int OwnerId { get; set; }
        
        [JsonPropertyName("postId")]
        public int PostId { get; set; }
        
        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }
    }
}