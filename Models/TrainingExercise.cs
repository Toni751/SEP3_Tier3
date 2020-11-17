namespace SEP3_Tier3.Models
{
    public class TrainingExercise
    {
        public int TrainingId { get; set; }
        public Training Training { get; set; }
        public int ExerciseId { get; set; }
        public Exercise Exercise { get; set; }
    }
}