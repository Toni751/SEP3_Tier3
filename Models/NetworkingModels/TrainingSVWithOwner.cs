using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class TrainingSVWithOwner : TrainingShortVersion
    {
        [JsonPropertyName("owner")]
        public SearchBarUser Owner { get; set; }
    }
}