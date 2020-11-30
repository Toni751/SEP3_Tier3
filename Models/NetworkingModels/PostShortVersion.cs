using System;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class PostShortVersion
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("title")]
        public string Title { get; set;}
        
        [JsonPropertyName("content")]
        public string Content { get; set;}
        
        [JsonPropertyName("owner")]
        public UserShortVersion Owner { get; set;}
        
        [JsonPropertyName("picture")]
        public byte[] Picture { get; set; }
        
        [JsonPropertyName("timeStamp")]
        public DateTime TimeStamp { get; set;}
        
        [JsonPropertyName("numberOfComments")]
        public int NumberOfComments { get; set; }
        
        [JsonPropertyName("numberOfLikes")]
        public int NumberOfLikes { get; set; }
        
        [JsonPropertyName("hasImage")]
        public bool HasImage { get; set; }
        
        //0: isLikedByUser, 1: isReportedByUser
        [JsonPropertyName("postStatus")]
        public bool[] PostStatus { get; set; }
    }
}