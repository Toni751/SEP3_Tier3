using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing the sockets version for the comment model
    /// </summary>
    public class CommentSockets
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("owner")]
        public UserShortVersion Owner { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }
        
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set; }
    }
}