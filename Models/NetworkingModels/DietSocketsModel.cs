using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing the sockets version of the diet model
    /// </summary>
    public class DietSocketsModel : DietShortVersion
    {
        [JsonPropertyName("meals")]
        public List<Meal> Meals { get; set; }
    }
}