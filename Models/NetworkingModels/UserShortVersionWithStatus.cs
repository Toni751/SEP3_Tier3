using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class UserShortVersionWithStatus : UserShortVersion
    {
        // 0: you shared trainings with receiver
        // 1: you shared diets with receiver
        [JsonPropertyName("status")]
        public bool[] Status { get; set; }
    }
}