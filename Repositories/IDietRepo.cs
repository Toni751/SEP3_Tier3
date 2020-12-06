using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    public interface IDietRepo
    {
        Task<int> AddDietAsync(DietSocketsModelWithOwner diet);
        Task<DietSocketsModelWithOwner> GetDietByIdAsync(int id);
        List<DietSVWithOwner> GetPublicDiets(int offset);
        List<DietShortVersion> GetPrivateDietsForUser(int userId, int offset);
        Task<bool> EditDietAsync(DietSocketsModel diet);
        Task<bool> DeleteDietAsync(int dietId);
        Task<int> AddMealToDietAsync(MealWithDiet meal);
        Task<bool> EditMealInDiet(MealWithDiet meal);
        Task<bool> DeleteMealFromDiet(int mealId, int dietId);
    }
}