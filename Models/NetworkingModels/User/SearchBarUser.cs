using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing users used in the search bar
    /// </summary>
    public class SearchBarUser
    {
        [JsonPropertyName("userId")]
        public int UserId { get; set; }
        
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
    }
}