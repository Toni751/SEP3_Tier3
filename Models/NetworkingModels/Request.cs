using System;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class Request
    {
        [JsonPropertyName("actionType")]
        public string ActionType { get; set; }
        
        [JsonPropertyName("argument")]
        public Object Argument { get; set; }
    }
}