using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;

namespace SEP3_Tier3.SocketControllers.Implementation
{
    public class DietSocket : IDietSocket
    {
        private IDietRepo dietRepo;

        public DietSocket(IDietRepo dietRepo)
        {
            this.dietRepo = dietRepo;
        }

        public async Task<ActualRequest> HandleClientRequest(ActualRequest actualRequest)
        {
            switch (actualRequest.Request.ActionType)
            {
                case "DIET_CREATE":
                    return await AddDietAsync(actualRequest);
                case "DIET_GET_BY_ID":
                    return await GetDietByIdAsync(actualRequest);
                case "DIET_GET_PUBLIC":
                    return GetPublicDiets(actualRequest);
                case "DIET_GET_PRIVATE":
                    return GetPrivateDietsForUser(actualRequest);
                case "DIET_EDIT":
                    return await EditDietAsync(actualRequest);
                case "DIET_DELETE":
                    return await DeleteDietAsync(actualRequest);
                case "DIET_ADD_MEAL":
                    return await AddMealToDietAsync(actualRequest);
                case "DIET_EDIT_MEAL":
                    return await EditMealInDietAsync(actualRequest);
                case "DIET_DELETE_MEAL":
                    return await DeleteMealFromDietAsync(actualRequest);
                default:
                    return null;
            }
        }

        private async Task<ActualRequest> AddDietAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            DietSocketsModelWithOwner diet = JsonSerializer.Deserialize<DietSocketsModelWithOwner>(request.Argument.ToString());
            int dietId = await dietRepo.AddDietAsync(diet);
            Request responseRequest = new Request
            {
                ActionType = ActionType.DIET_CREATE.ToString(),
                Argument = JsonSerializer.Serialize(dietId)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> GetDietByIdAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int dietId = Convert.ToInt32(request.Argument.ToString());
            DietSocketsModelWithOwner diet = await dietRepo.GetDietByIdAsync(dietId);
            Request responseRequest = new Request
            {
                ActionType = ActionType.DIET_GET_BY_ID.ToString(),
                Argument = JsonSerializer.Serialize(diet)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private ActualRequest GetPublicDiets(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int offset = Convert.ToInt32(request.Argument.ToString());
            List<DietSVWithOwner> publicDiets = dietRepo.GetPublicDiets(offset);
            Request responseRequest = new Request
            {
                ActionType = ActionType.DIET_GET_PUBLIC.ToString(),
                Argument = JsonSerializer.Serialize(publicDiets)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private ActualRequest GetPrivateDietsForUser(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> ints = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<DietShortVersion> privateDiets = dietRepo.GetPrivateDietsForUser(ints[0], ints[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.DIET_GET_PRIVATE.ToString(),
                Argument = JsonSerializer.Serialize(privateDiets)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> EditDietAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            DietSocketsModel diet = JsonSerializer.Deserialize<DietSocketsModel>(request.Argument.ToString());
            bool editResult = await dietRepo.EditDietAsync(diet);
            Request responseRequest = new Request
            {
                ActionType = ActionType.DIET_EDIT.ToString(),
                Argument = JsonSerializer.Serialize(editResult)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> DeleteDietAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int dietId = Convert.ToInt32(request.Argument.ToString());
            Console.WriteLine("Deleting diet with id " + dietId);
            bool response = await dietRepo.DeleteDietAsync(dietId);
            Request responseRequest = new Request
            {
                ActionType = ActionType.DIET_DELETE.ToString(),
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> AddMealToDietAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            MealWithDiet meal = JsonSerializer.Deserialize<MealWithDiet>(request.Argument.ToString());
            int mealId = await dietRepo.AddMealToDietAsync(meal);
            Request responseRequest = new Request
            {
                ActionType = ActionType.DIET_ADD_MEAL.ToString(),
                Argument = JsonSerializer.Serialize(mealId)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> EditMealInDietAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            MealWithDiet meal = JsonSerializer.Deserialize<MealWithDiet>(request.Argument.ToString());
            bool editResult = await dietRepo.EditMealInDiet(meal);
            Request responseRequest = new Request
            {
                ActionType = ActionType.DIET_EDIT_MEAL.ToString(),
                Argument = JsonSerializer.Serialize(editResult)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> DeleteMealFromDietAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> ints = JsonSerializer.Deserialize<List<int>>(request.Argument.ToString());
            Console.WriteLine("Deleting meal with id " + ints[0] + " from exercise " + ints[1]);
            bool response = await dietRepo.DeleteMealFromDiet(ints[0], ints[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.DIET_DELETE_MEAL.ToString(),
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }
    }
}