using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing an exercise with the id of the training it belongs to
    /// </summary>
    public class ExerciseWithTraining : Exercise
    {
        [JsonPropertyName("trainingId")]
        public int TrainingId { get; set; }
    }
}