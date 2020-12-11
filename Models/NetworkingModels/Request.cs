using System;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class user for storing sockets requests and responses, with the type and argument
    /// </summary>
    public class Request
    {
        [JsonPropertyName("actionType")]
        public string ActionType { get; set; }
        
        [JsonPropertyName("argument")]
        public Object Argument { get; set; }
    }
}