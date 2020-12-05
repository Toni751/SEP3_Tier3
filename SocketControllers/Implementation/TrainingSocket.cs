using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;

namespace SEP3_Tier3.SocketControllers.Implementation
{
    public class TrainingSocket : ITrainingSocket
    {
        private ITrainingRepo trainingRepo;

        public TrainingSocket(ITrainingRepo trainingRepo)
        {
            this.trainingRepo = trainingRepo;
        }

        public async Task<ActualRequest> HandleClientRequest(ActualRequest actualRequest)
        {
            switch (actualRequest.Request.ActionType)
            {
                case "TRAINING_CREATE":
                    return await AddTrainingAsync(actualRequest);
                case "TRAINING_GET_BY_ID":
                    return await GetTrainingById(actualRequest);
                case "TRAINING_GET_PUBLIC":
                    return GetPublicTrainings(actualRequest);
                case "TRAINING_GET_PRIVATE":
                    return GetPrivateTrainingsForUser(actualRequest);
                case "TRAINING_GET_USER":
                    return GetTrainingsForUser(actualRequest);
                case "TRAINING_GET_WEEK":
                    return GetTrainingsInWeekForUser(actualRequest);
                case "TRAINING_EDIT":
                    return await EditTrainingAsync(actualRequest);
                case "TRAINING_DELETE":
                    return await DeleteTrainingAsync(actualRequest);
                case "TRAINING_ADD_EXERCISE":
                    return await AddExerciseToTrainingAsync(actualRequest);
                case "TRAINING_EDIT_EXERCISE":
                    return await EditExerciseInTrainingAsync(actualRequest);
                case "TRAINING_DELETE_EXERCISE":
                    return await DeleteExerciseFromTrainingAsync(actualRequest);
                default:
                    return null;
            }
        }

        private async Task<ActualRequest> AddTrainingAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            TrainingSocketsModelWithOwner training = JsonSerializer.Deserialize<TrainingSocketsModelWithOwner>(request.Argument.ToString());
            Console.WriteLine("XXXXXXXXXXXXXXXXXXX" + training.TimeStamp);
            int trainingId = await trainingRepo.AddTrainingAsync(training);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_CREATE.ToString(),
                Argument = JsonSerializer.Serialize(trainingId)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> GetTrainingById(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int trainingId = Convert.ToInt32(request.Argument.ToString());
            TrainingSocketsModelWithOwner training = await trainingRepo.GetTrainingByIdAsync(trainingId);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_GET_BY_ID.ToString(),
                Argument = JsonSerializer.Serialize(training)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private ActualRequest GetPublicTrainings(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int offset = Convert.ToInt32(request.Argument.ToString());
            List<TrainingSVWithOwner> publicTrainings = trainingRepo.GetPublicTrainings(offset);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_GET_PUBLIC.ToString(),
                Argument = JsonSerializer.Serialize(publicTrainings)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private ActualRequest GetPrivateTrainingsForUser(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> ints = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<TrainingShortVersion> privateTrainings = trainingRepo.GetPrivateTrainingsForUser(ints[0], ints[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_GET_PRIVATE.ToString(),
                Argument = JsonSerializer.Serialize(privateTrainings)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private ActualRequest GetTrainingsForUser(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> ints = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<TrainingSVWithOwner> trainingsForUser = trainingRepo.GetTrainingsForUser(ints[0], ints[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_GET_USER.ToString(),
                Argument = JsonSerializer.Serialize(trainingsForUser)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private ActualRequest GetTrainingsInWeekForUser(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> ints = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<TrainingSVWithTime> trainingsInWeekForUser = trainingRepo.GetTrainingsInWeekForUser(ints[0], ints[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_GET_WEEK.ToString(),
                Argument = JsonSerializer.Serialize(trainingsInWeekForUser)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> EditTrainingAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            TrainingSocketsModel training = JsonSerializer.Deserialize<TrainingSocketsModel>(request.Argument.ToString());
            bool editResult = await trainingRepo.EditTrainingAsync(training);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_EDIT.ToString(),
                Argument = JsonSerializer.Serialize(editResult)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> DeleteTrainingAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int trainingId = Convert.ToInt32(request.Argument.ToString());
            Console.WriteLine("Deleting training with id " + trainingId);
            bool response = await trainingRepo.DeleteTrainingAsync(trainingId);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_DELETE.ToString(),
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> AddExerciseToTrainingAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            ExerciseWithTraining exercise =
                JsonSerializer.Deserialize<ExerciseWithTraining>(request.Argument.ToString());
            int exerciseId = await trainingRepo.AddExerciseToTrainingAsync(exercise);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_ADD_EXERCISE.ToString(),
                Argument = JsonSerializer.Serialize(exerciseId)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> EditExerciseInTrainingAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            ExerciseWithTraining exercise = JsonSerializer.Deserialize<ExerciseWithTraining>(request.Argument.ToString());
            bool editResult = await trainingRepo.EditExerciseInTrainingAsync(exercise);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_EDIT_EXERCISE.ToString(),
                Argument = JsonSerializer.Serialize(editResult)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> DeleteExerciseFromTrainingAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> ints = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            Console.WriteLine("Deleting exercise with id " + ints[0] + " from training " + ints[1]);
            bool response = await trainingRepo.DeleteExerciseFromTrainingAsync(ints[0], ints[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.TRAINING_DELETE_EXERCISE.ToString(),
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