using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    public interface ITrainingRepo
    {
        Task<int> AddTrainingAsync(TrainingSocketsModelWithOwner training);
        Task<TrainingSocketsModelWithOwner> GetTrainingByIdAsync(int id);
        List<TrainingSVWithOwner> GetPublicTrainings(int offset);
        List<TrainingShortVersion> GetPrivateTrainingsForUser(int userId, int offset);
        List<TrainingSVWithOwner> GetTrainingsForUser(int id, int offset);
        List<TrainingSVWithTime> GetTrainingsInWeekForUser(int userId, int weekNumber);
        List<TrainingSVWithTime> GetTrainingsTodayForUser(int userId);
        Task<bool> EditTrainingAsync(TrainingSocketsModel training);
        Task<bool> DeleteTrainingAsync(int trainingId);
        Task<int> AddExerciseToTrainingAsync(ExerciseWithTraining exercise);
        Task<bool> EditExerciseInTrainingAsync(ExerciseWithTraining exercise);
        Task<bool> DeleteExerciseFromTrainingAsync(int exerciseId, int trainingId);
    }
}