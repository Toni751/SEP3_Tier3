using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a short version of a training
    /// </summary>
    public class TrainingShortVersion
    {
        [JsonPropertyName("trainingId")]
        public int TrainingId { get; set; }
        
        [JsonPropertyName("trainingTitle")]
        public string TrainingTitle { get; set; }
    }
}