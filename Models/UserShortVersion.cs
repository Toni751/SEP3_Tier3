using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SEP3_T3.Models
{
    public class UserShortVersion
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }

        [JsonPropertyName("accountType")]
        public string AccountType { get; set; }
        
        [JsonPropertyName("userFullName")]
        public string UserFullName { get; set; }

        [JsonPropertyName("avatar")]
        public byte[] Avatar { get; set; }
    }
}