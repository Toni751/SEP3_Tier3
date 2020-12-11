using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    /// <summary>
    /// Interface storing the functionality of the diet repository class
    /// </summary>
    public interface IDietRepo
    {
        /// <summary>
        /// Persists a given diet to the database
        /// </summary>
        /// <param name="diet">the diet to be added</param>
        /// <returns>the id of the created diet</returns>
        Task<int> AddDietAsync(DietSocketsModelWithOwner diet);
        
        /// <summary>
        /// Retrieves a diet with its owner, by id
        /// </summary>
        /// <param name="id">the id of the diet</param>
        /// <returns>the diet with its owner</returns>
        Task<DietSocketsModelWithOwner> GetDietByIdAsync(int id);
        
        /// <summary>
        /// Retrieves a list with public diets
        /// </summary>
        /// <param name="offset">the number of diets to be skipped</param>
        /// <returns>a list with public diets</returns>
        List<DietSVWithOwner> GetPublicDiets(int offset);
        
        /// <summary>
        /// Retrieves a list of private diets belonging to a given user
        /// </summary>
        /// <param name="userId">the id of the given user</param>
        /// <param name="offset">the number of diets to be skipped</param>
        /// <returns>a list of private diets belonging to a given user</returns>
        List<DietShortVersion> GetPrivateDietsForUser(int userId, int offset);
        
        /// <summary>
        /// Edits a given diet
        /// </summary>
        /// <param name="diet">the new value for the diet</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> EditDietAsync(DietSocketsModel diet);
        
        /// <summary>
        /// Deletes a diet with a given id
        /// </summary>
        /// <param name="dietId">the id of the diet</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> DeleteDietAsync(int dietId);
        
        /// <summary>
        /// Adds a given meal to a given diet
        /// </summary>
        /// <param name="meal">the meal to be added, with the diet it belongs to</param>
        /// <returns>the id of the created meal</returns>
        Task<int> AddMealToDietAsync(MealWithDiet meal);
        
        /// <summary>
        /// Edits a given meal in a given diet
        /// </summary>
        /// <param name="meal">the meal to be edited, with the corresponding diet</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> EditMealInDiet(MealWithDiet meal);
        
        /// <summary>
        /// Deletes a given meal from a given diet
        /// </summary>
        /// <param name="mealId">the id of the meal</param>
        /// <param name="dietId">the id of the diet</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> DeleteMealFromDiet(int mealId, int dietId);
    }
}