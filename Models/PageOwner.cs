using System.Text.Json.Serialization;

namespace SEP3_T3.Models
{
    public class PageOwner : RegularUser
    {
        [JsonPropertyName("address")]
        public Address Address { get; set; }
    }
}