using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a training short version with its owner
    /// </summary>
    public class TrainingSVWithOwner : TrainingShortVersion
    {
        [JsonPropertyName("owner")]
        public SearchBarUser Owner { get; set; }
    }
}