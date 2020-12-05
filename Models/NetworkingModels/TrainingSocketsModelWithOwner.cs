using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class TrainingSocketsModelWithOwner : TrainingSocketsModel
    {
        [JsonPropertyName("owner")]
        public SearchBarUser Owner { get; set; }
    }
}