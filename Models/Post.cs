﻿using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SEP3_T3.Models
{
    public class Post //add owner I think
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
        
        [JsonPropertyName("comments")]
        public List<Comment> Comments { get; set;}
        
        [JsonPropertyName("userIdsForLikes")]
        public List<int> UserIdsForLikes{ get; set;}
    }
}