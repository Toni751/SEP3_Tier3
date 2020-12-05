using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class ExerciseWithTraining : Exercise
    {
        [JsonPropertyName("trainingId")]
        public int TrainingId { get; set; }
    }
}