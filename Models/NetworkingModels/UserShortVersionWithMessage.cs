using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class UserShortVersionWithMessage : UserShortVersion
    {
        [JsonPropertyName("message")]
        public MessageSocketsModel Message { get; set; }
    }
}