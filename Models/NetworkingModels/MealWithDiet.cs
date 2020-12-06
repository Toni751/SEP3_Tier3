using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    public class MealWithDiet : Meal
    {
        [JsonPropertyName("dietId")]
        public int DietId { get; set; }
    }
}