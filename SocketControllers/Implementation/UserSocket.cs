using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.Json;
using System.Threading.Tasks;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;
using SkiaSharp;

namespace SEP3_Tier3.SocketControllers.Implementation
{
    public class UserSocket : IUserSocket
    {
        private IUserRepo userRepo;
        private readonly string FILE_PATH;

        public UserSocket(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
            FILE_PATH = ImagesUtil.FILE_PATH;
        }

        public async Task<ActualRequest> HandleClientRequest(ActualRequest actualRequest)
        {
            switch (actualRequest.Request.ActionType)
            {
                case "USER_REGISTER":
                    return await AddUserAsync(actualRequest);
                case "USER_LOGIN":
                    return await LoginAsync(actualRequest);
                case "USER_GET_BY_ID":
                    return await GetUserByIdAsync(actualRequest);
                case "USER_EDIT":
                    return await UpdateUserAsync(actualRequest);
                case "USER_DELETE":
                    return await DeleteUserAsync(actualRequest);
                case "USER_FRIEND_REQUEST_SEND":
                case "USER_FRIEND_REQUEST_RESPONSE":
                case "USER_FRIEND_REMOVE":
                case "USER_SHARE_TRAININGS":
                case "USER_SHARE_DIETS":
                case "USER_FOLLOW_PAGE":
                case "USER_RATE_PAGE":
                case "USER_REPORT":
                    return await PostUserAction(actualRequest);
                default:
                    return null;
            }
        }

        private async Task<ActualRequest> PostUserAction(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            UserActionSockets userActionSockets =
                JsonSerializer.Deserialize<UserActionSockets>(request.Argument.ToString());
            Console.WriteLine("Posting user action " + userActionSockets.ActionType);
            bool response;
            if (userActionSockets.ActionType.Equals("USER_FRIEND_REMOVE"))
                response = await userRepo.RemoveFriendshipAsync(userActionSockets);
            else if (userActionSockets.ActionType.Equals("USER_RATE_PAGE"))
                response = await userRepo.PostPageRatingAsync(userActionSockets);
            else
                response = await userRepo.PostUserActionAsync(userActionSockets);
            Request responseRequest = new Request
            {
                ActionType = userActionSockets.ActionType,
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> DeleteUserAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int userId = Convert.ToInt32(request.Argument.ToString());
            Console.WriteLine("Deleting user with id " + userId);
            bool response = await userRepo.DeleteUserAsync(userId);
            if (response)
                ImagesUtil.DeleteUserFolder($"{FILE_PATH}/Users/{userId}");

            Request responseRequest = new Request
            {
                ActionType = ActionType.USER_DELETE.ToString(),
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private async Task<ActualRequest> AddUserAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            string userAsJson = request.Argument.ToString();
            UserSocketsModel user = JsonSerializer.Deserialize<UserSocketsModel>(userAsJson);
            Console.WriteLine("Server got register user " + JsonSerializer.Serialize(user));
            int result = await userRepo.AddUserAsync(user);
            bool resultBool = result >= 0;
            Request requestResponse = new Request
            {
                ActionType = ActionType.USER_REGISTER.ToString(),
                Argument = JsonSerializer.Serialize(resultBool)
            };
            if (result >= 0)
            {
                try
                {
                    Console.WriteLine($"************* {FILE_PATH}/Users/defaultAvatar.jpg");

                    byte[] readDefaultAvatar = File.ReadAllBytes($"{FILE_PATH}/Users/defaultAvatar.jpg");

                    Console.WriteLine($"************* After first");

                    byte[] readDefaultBg = File.ReadAllBytes($"{FILE_PATH}/Users/defaultBg.jpg");


                    Console.WriteLine($"************* After second");

                    ImagesUtil.WriteImageToPath(readDefaultAvatar, $"{FILE_PATH}/Users/{result}", "/avatar.jpg");
                    ImagesUtil.WriteImageToPath(readDefaultBg, $"{FILE_PATH}/Users/{result}", "/background.jpg");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Default avatar not found");
                }
            }

            return new ActualRequest
            {
                Request = requestResponse,
                Images = null
            };
        }

        private async Task<ActualRequest> LoginAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            string credentialsAsJson = request.Argument.ToString();
            LoginCredentials loginCredentials = JsonSerializer.Deserialize<LoginCredentials>(credentialsAsJson);
            Console.WriteLine("Got login credentials " + loginCredentials.Email + loginCredentials.Password);
            UserShortVersion loginResult = await userRepo.LoginAsync(loginCredentials.Email, loginCredentials.Password);
            Request requestResponse = new Request
            {
                ActionType = ActionType.USER_LOGIN.ToString(),
                Argument = JsonSerializer.Serialize(loginResult)
            };
            if (loginResult == null)
                return new ActualRequest
                {
                    Request = requestResponse,
                    Images = null
                };
            List<byte[]> images = new List<byte[]>();
            if (!loginResult.AccountType.Equals("Administrator"))
            {
                try
                {
                    byte[] readFile = File.ReadAllBytes($"{FILE_PATH}/Users/{loginResult.UserId}/avatar.jpg");
                    images.Add(readFile);
                }
                catch (Exception e)
                {
                    Console.WriteLine("No avatar found for user " + loginResult.UserId);
                }
            }

            return new ActualRequest
            {
                Request = requestResponse,
                Images = images
            };
        }

        private async Task<List<Post>> GetLatestPostsForUserAsync(Request request)
        {
            throw new System.NotImplementedException();
        }

        private async Task<ActualRequest> GetUserByIdAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            Console.WriteLine("Get user argument " + request.Argument);
            List<int> userIds = JsonSerializer.Deserialize<List<int>>(request.Argument.ToString());
            Console.WriteLine("Retrieving user with id " + userIds[1] + " by " + userIds[0]);
            UserSocketsModel user = await userRepo.GetUserByIdAsync(userIds[0], userIds[1]);
            Request requestResponse = new Request
            {
                ActionType = ActionType.USER_GET_BY_ID.ToString(),
                Argument = JsonSerializer.Serialize(user)
            };
            List<byte[]> images = new List<byte[]>();
            try
            {
                // /Images/users/{userId}/....
                // /Images/posts/{postId}/....
                var readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/Users/{user.Id}/avatar.jpg");
                images.Add(ImagesUtil.ResizeImage(readAvatarFile, 200, 200));
                var readBackgroundFile = File.ReadAllBytes($"{FILE_PATH}/Users/{user.Id}/background.jpg");
                images.Add(readBackgroundFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("No avatar found for user " + user.Id);
            }

            return new ActualRequest
            {
                Request = requestResponse,
                Images = images
            };
        }

        private async Task<ActualRequest> UpdateUserAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;

            Console.WriteLine("********:" + request.Argument.ToString());

            UserSocketsModel user = JsonSerializer.Deserialize<UserSocketsModel>(request.Argument.ToString());
            bool result = await userRepo.EditUserAsync(user);
            Request requestResponse = new Request
            {
                ActionType = ActionType.USER_EDIT.ToString(),
                Argument = JsonSerializer.Serialize(result)
            };
            if (actualRequest.Images != null && actualRequest.Images.Any())
            {
                //if (actualRequest.Images[0])
                //{
                if (user.Email != null)
                {
                     ImagesUtil.WriteImageToPath(actualRequest.Images[0], $"{FILE_PATH}/Users/{user.Id}", "/avatar.jpg");
                     Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%: only avatar");
                }
                else
                {
                      ImagesUtil.WriteImageToPath(actualRequest.Images[0], $"{FILE_PATH}/Users/{user.Id}", "/background.jpg");
                      Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%: only bg");
                }
                   
                    // if (user.ProfileBackground.Length > 2)
                    // {
                         // ImagesUtil.WriteImageToPath(actualRequest.Images[1], $"{FILE_PATH}/Users/{result}", "/background.jpg");
                         // Console.WriteLine("%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%%: bg and avatar");
                    // }
                       
                // }
                // else if (user.ProfileBackground.Length > 2)
                // {
                    
                //}
                  
            }

            return new ActualRequest
            {
                Request = requestResponse,
                Images = null
            };
        }
    }
}