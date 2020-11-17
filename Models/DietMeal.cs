namespace SEP3_Tier3.Models
{
    public class DietMeal
    {
        public int MealId { get; set; }
        public Meal Meal { get; set; }
        public int DietId { get; set; }
        public Diet Diet { get; set; }
    }
}