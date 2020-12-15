using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a training with it's owner
    /// </summary>
    public class TrainingSocketsModelWithOwner : TrainingSocketsModel
    {
        [JsonPropertyName("owner")]
        public SearchBarUser Owner { get; set; }
    }
}