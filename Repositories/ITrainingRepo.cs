using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    /// <summary>
    /// Interface storing the functionality of the training repository class
    /// </summary>
    public interface ITrainingRepo
    {
        /// <summary>
        /// Persists a given training to the database
        /// </summary>
        /// <param name="training">the training to be added</param>
        /// <returns>the id of the created training</returns>
        Task<int> AddTrainingAsync(TrainingSocketsModelWithOwner training);
        
        /// <summary>
        /// Retrieves a training with its owner, by id
        /// </summary>
        /// <param name="id">the id of the training</param>
        /// <returns>the training with its owner</returns>
        Task<TrainingSocketsModelWithOwner> GetTrainingByIdAsync(int id);
        
        /// <summary>
        /// Retrieves a list with public trainings
        /// </summary>
        /// <param name="offset">the number of trainings to be skipped</param>
        /// <returns>a list with public trainings</returns>
        List<TrainingSVWithOwner> GetPublicTrainings(int offset);
        
        /// <summary>
        /// Retrieves a list of private trainings belonging to a given user
        /// </summary>
        /// <param name="userId">the id of the given user</param>
        /// <param name="offset">the number of diets to be skipped</param>
        /// <returns>a list of private trainings belonging to a given user</returns>
        List<TrainingShortVersion> GetPrivateTrainingsForUser(int userId, int offset);
        
        /// <summary>
        /// Retrieves a list of trainings for a given user(public + his own private trainings)
        /// </summary>
        /// <param name="id">the id of the given user</param>
        /// <param name="offset">the number of diets to be skipped</param>
        /// <returns>a list of trainings for the a given user</returns>
        List<TrainingSVWithOwner> GetTrainingsForUser(int id, int offset);
        
        /// <summary>
        /// Retrieves a list of trainings in a given week for a given user
        /// </summary>
        /// <param name="userId">the id of the given user</param>
        /// <param name="weekNumber">the week number</param>
        /// <returns>a list of trainings in a given week for a given user</returns>
        List<TrainingSVWithTime> GetTrainingsInWeekForUser(int userId, int weekNumber);
        
        /// <summary>
        /// Retrieves a list of trainings in the current for a given user
        /// </summary>
        /// <param name="userId">the id of the given user</param>
        /// <returns>a list of trainings in the current for a given user</returns>
        List<TrainingSVWithTime> GetTrainingsTodayForUser(int userId);
        
        /// <summary>
        /// Edits a given training
        /// </summary>
        /// <param name="training">the new value for the training</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> EditTrainingAsync(TrainingSocketsModel training);
        
        /// <summary>
        /// Deletes a training with a given id
        /// </summary>
        /// <param name="trainingId">the id of the training</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> DeleteTrainingAsync(int trainingId);
        
        /// <summary>
        /// Adds a given exercise to a given training
        /// </summary>
        /// <param name="exercise">the exercise to be added, with the training it belongs to</param>
        /// <returns>the id of the created exercise</returns>
        Task<int> AddExerciseToTrainingAsync(ExerciseWithTraining exercise);
        
        /// <summary>
        /// Edits a given exercise in a given training
        /// </summary>
        /// <param name="exercise">the exercise to be edited, with the training it belongs to</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> EditExerciseInTrainingAsync(ExerciseWithTraining exercise);
        
        /// <summary>
        /// Deletes a given exercise from a given training
        /// </summary>
        /// <param name="exerciseId">the id of the exercise</param>
        /// <param name="trainingId">the id of the training</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> DeleteExerciseFromTrainingAsync(int exerciseId, int trainingId);
    }
}