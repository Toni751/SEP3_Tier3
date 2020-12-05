using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class TrainingShortVersion
    {
        [JsonPropertyName("trainingId")]
        public int TrainingId { get; set; }
        
        [JsonPropertyName("trainingTitle")]
        public string TrainingTitle { get; set; }
    }
}