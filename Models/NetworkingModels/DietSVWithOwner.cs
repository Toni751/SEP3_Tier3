using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class DietSVWithOwner : DietShortVersion
    {
        [JsonPropertyName("owner")]
        public SearchBarUser Owner { get; set; }
    }
}