using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing the sockets version of a diet with its owner
    /// </summary>
    public class DietSocketsModelWithOwner : DietSocketsModel
    {
        [JsonPropertyName("owner")]
        public SearchBarUser Owner { get; set; }
    }
}