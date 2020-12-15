using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a diet without meals and with the owner
    /// </summary>
    public class DietSVWithOwner : DietShortVersion
    {
        [JsonPropertyName("owner")]
        public SearchBarUser Owner { get; set; }
    }
}