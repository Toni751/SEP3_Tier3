using System.Text.Json.Serialization;

namespace SEP3_Tier3.Models
{
    /// <summary>
    /// Class for storing a meal with the diet it belongs to
    /// </summary>
    public class MealWithDiet : Meal
    {
        [JsonPropertyName("dietId")]
        public int DietId { get; set; }
    }
}