using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class DietSocketsModelWithOwner : DietSocketsModel
    {
        [JsonPropertyName("owner")]
        public SearchBarUser Owner { get; set; }
    }
}