using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class DietSocketsModel : DietShortVersion
    {
        [JsonPropertyName("meals")]
        public List<Meal> Meals { get; set; }
    }
}