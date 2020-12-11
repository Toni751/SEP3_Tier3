using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a user short version with the corresponding fitness status
    /// </summary>
    public class UserShortVersionWithStatus : UserShortVersion
    {
        // 0: you shared trainings with receiver
        // 1: you shared diets with receiver
        [JsonPropertyName("status")]
        public bool[] Status { get; set; }
    }
}