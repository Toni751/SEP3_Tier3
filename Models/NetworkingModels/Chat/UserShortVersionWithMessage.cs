using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a user short version with the last message sent by the user
    /// </summary>
    public class UserShortVersionWithMessage : UserShortVersion
    {
        [JsonPropertyName("message")]
        public MessageSocketsModel Message { get; set; }
    }
}