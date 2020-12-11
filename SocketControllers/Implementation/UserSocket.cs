using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text.Json;
using System.Threading.Tasks;
using Org.BouncyCastle.Ocsp;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;
using SkiaSharp;

namespace SEP3_Tier3.SocketControllers.Implementation
{
    /// <summary>
    /// Class for handling user-related sockets requests
    /// </summary>
    public class UserSocket : IUserSocket
    {
        private IUserRepo userRepo;
        private readonly string FILE_PATH;

        /// <summary>
        /// One-argument constructor initializing the user repository
        /// </summary>
        /// <param name="userRepo">the given value for the user repo</param>
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
                    return await PostUserActionAsync(actualRequest);
                case "USER_DELETE_NOTIFICATION":
                    return await DeleteNotificationAsync(actualRequest);
                case "USER_FILTER":
                    return GetUsersByFilter(actualRequest);
                case "USER_GET_GYMS":
                    return GetGymsByCity(actualRequest);
                case "USER_GET_NOTIFICATIONS":
                    return GetNotificationsForUser(actualRequest);
                case "USER_GET_FRIENDS":
                    return GetFriendsForUser(actualRequest);
                case "USER_INCREMENT_SCORE":
                    return await IncrementUserScoreAsync(actualRequest);
                case "USER_GET_ONLINE_FRIENDS":
                    return GetOnlineFriendsForUser(actualRequest);
                case "USER_LOGOUTORIN":
                    return LogoutUser(actualRequest);
                case "USER_GET_SV_BY_ID":
                    return GetUserShortVersionById(actualRequest);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Retrieves a user short version instance with the given user id
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest GetUserShortVersionById(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int userId = Convert.ToInt32(request.Argument.ToString());
            UserShortVersion user = userRepo.GetUserShortVersionById(userId);
            Request response = new Request
            {
                ActionType = ActionType.USER_GET_SV_BY_ID.ToString(),
                Argument = JsonSerializer.Serialize(user)
            };
            List<byte[]> userAvatars = new List<byte[]>();
            if (user != null)
            {
                try {
                    var readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/Users/{user.UserId}/avatar.jpg");
                    userAvatars.Add(ImagesUtil.ResizeImage(readAvatarFile, 20, 20));
                }
                catch (Exception e) {
                    Console.WriteLine("No avatar found for user " + user.UserId);
                }
            }
            return new ActualRequest
            {
                Request = response,
                Images = userAvatars
            };
        }

        /// <summary>
        /// Logs in our out a user and returns a list with his online friends ids, so that they can be notified
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest LogoutUser(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> userInts = JsonSerializer.Deserialize<List<int>>(request.Argument.ToString());
            List<int> onlineFriendIds = userRepo.LogoutOrInUser(userInts[0], userInts[1] == 1);
            Request response = new Request
            {
                ActionType = ActionType.USER_LOGOUTORIN.ToString(),
                Argument = JsonSerializer.Serialize(onlineFriendIds)
            };
            return new ActualRequest
            {
                Request = response,
                Images = null
            };
        }

        /// <summary>
        /// Gets a list with all the online friends belonging to a given user
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest GetOnlineFriendsForUser(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int userId = Convert.ToInt32(request.Argument.ToString());
            List<UserShortVersion> onlineFriends = userRepo.GetOnlineFriendsForUser(userId);
            Request response = new Request
            {
                ActionType = ActionType.USER_GET_ONLINE_FRIENDS.ToString(),
                Argument = JsonSerializer.Serialize(onlineFriends)
            };
            List<byte[]> userAvatars = new List<byte[]>();
            if (onlineFriends != null && onlineFriends.Count > 0) {
                foreach (var friend in onlineFriends) {
                    try {
                        var readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/Users/{friend.UserId}/avatar.jpg");
                        userAvatars.Add(ImagesUtil.ResizeImage(readAvatarFile, 20, 20));
                    }
                    catch (Exception e) {
                        Console.WriteLine("No avatar found for user " + friend.UserId);
                    }
                }
            }
            return new ActualRequest
            {
                Request = response,
                Images = userAvatars
            };
        }

        /// <summary>
        /// Increments a given user's score by a given amount
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> IncrementUserScoreAsync(ActualRequest actualRequest)
        {
            List<int> integers = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            bool response = await userRepo.IncrementUserScoreAsync(integers[0], integers[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.USER_INCREMENT_SCORE.ToString(),
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        /// <summary>
        /// Retrieves a list of the target user's friends, or common friends with the sender
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest GetFriendsForUser(ActualRequest actualRequest)
        {
            List<int> integers = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<UserShortVersionWithStatus> friends = userRepo.GetFriendsForUser(integers[0], integers[1], integers[2]);
            Request response = new Request
            {
                ActionType = ActionType.USER_GET_FRIENDS.ToString(),
                Argument = JsonSerializer.Serialize(friends)
            };
            List<byte[]> userAvatars = new List<byte[]>();
            if (friends != null && friends.Count > 0) {
                foreach (var friend in friends) {
                    try {
                        var readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/Users/{friend.UserId}/avatar.jpg");
                        userAvatars.Add(ImagesUtil.ResizeImage(readAvatarFile, 20, 20));
                    }
                    catch (Exception e) {
                        Console.WriteLine("No avatar found for user " + friend.UserId);
                    }
                }
            }
            return new ActualRequest
            {
                Request = response,
                Images = userAvatars
            };
        }

        /// <summary>
        /// Gets a list with all the notifications belonging to a given user
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest GetNotificationsForUser(ActualRequest actualRequest)
        {
            int userId = Convert.ToInt32(actualRequest.Request.Argument.ToString());
            List<NotificationSockets> notifications = userRepo.GetNotificationsForUser(userId);
            Request response = new Request
            {
                ActionType = ActionType.USER_GET_NOTIFICATIONS.ToString(),
                Argument = JsonSerializer.Serialize(notifications)
            };
            return new ActualRequest
            {
                Request = response,
                Images = null
            };
        }

        /// <summary>
        /// Returns a list with all the gyms in a given city
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest GetGymsByCity(ActualRequest actualRequest)
        {
            string city = actualRequest.Request.Argument.ToString();
            List<UserShortVersion> users = userRepo.GetAllGymsInCity(city);
            Request response = new Request
            {
                ActionType = ActionType.USER_GET_GYMS.ToString(),
                Argument = JsonSerializer.Serialize(users)
            };
            List<byte[]> userAvatars = new List<byte[]>();
            if (users != null && users.Count > 0) {
                foreach (var user in users) {
                    try {
                        var readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/Users/{user.UserId}/avatar.jpg");
                        userAvatars.Add(ImagesUtil.ResizeImage(readAvatarFile, 20, 20));
                    }
                    catch (Exception e) {
                        Console.WriteLine("No avatar found for user " + user.UserId);
                    }
                }
            }
            return new ActualRequest
            {
                Request = response,
                Images = userAvatars
            };
        }

        /// <summary>
        /// Retrieves the users whose username starts with the given filter string, if any
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest GetUsersByFilter(ActualRequest actualRequest)
        {
            string filterString = actualRequest.Request.Argument.ToString();
            List<SearchBarUser> filteredUsers = userRepo.GetUsersByFilter(filterString);
            Request responseRequest = new Request
            {
                ActionType = ActionType.USER_FILTER.ToString(),
                Argument = JsonSerializer.Serialize(filteredUsers)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }
        /// <summary>
        /// Deletes a notification with the given id
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> DeleteNotificationAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int notificationId = Convert.ToInt32(request.Argument.ToString());
            bool response = await userRepo.DeleteNotificationAsync(notificationId);
            Request responseRequest = new Request
            {
                ActionType = ActionType.USER_DELETE_NOTIFICATION.ToString(),
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }
        /// <summary>
        /// Creates a new user action and its corresponding notification, if any
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> PostUserActionAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            ModelActionSockets modelActionSockets =
                JsonSerializer.Deserialize<ModelActionSockets>(request.Argument.ToString());
            Console.WriteLine("Posting user action " + modelActionSockets.ActionType);
            int notificationId;
            if (modelActionSockets.ActionType.Equals("USER_FRIEND_REMOVE"))
                notificationId = await userRepo.RemoveFriendshipAsync(modelActionSockets);
            else if (modelActionSockets.ActionType.Equals("USER_RATE_PAGE"))
                notificationId = await userRepo.PostPageRatingAsync(modelActionSockets);
            else
                notificationId = await userRepo.PostUserActionAsync(modelActionSockets);

            Request responseRequest = new Request
            {
                ActionType = modelActionSockets.ActionType,
                Argument = JsonSerializer.Serialize(notificationId)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        /// <summary>
        /// Deletes a user with a given id
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> DeleteUserAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int userId = Convert.ToInt32(request.Argument.ToString());
            Console.WriteLine("Deleting user with id " + userId);
            List<int> postIdsForUser = userRepo.GetPostIdsForUser(userId);
            bool response = await userRepo.DeleteUserAsync(userId);
            if (response)
            {
                ImagesUtil.DeleteUserFolder($"{FILE_PATH}/Users/{userId}");
                foreach (var postId in postIdsForUser)
                {
                    ImagesUtil.DeleteFile($"{FILE_PATH}/Posts", $"{postId}.jpg");
                }
            }

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

        /// <summary>
        /// Persists a given user to the database
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
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
                    byte[] readDefaultAvatar = File.ReadAllBytes($"{FILE_PATH}/Users/defaultAvatar.jpg");
                    byte[] readDefaultBg = File.ReadAllBytes($"{FILE_PATH}/Users/defaultBg.jpg");
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

        /// <summary>
        /// Retrieves the user with the given email and password, if any
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
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

        /// <summary>
        /// Retrieves a user by id, and his status regarding the sender
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
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

        /// <summary>
        /// Edits a given user
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> UpdateUserAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;

            UserSocketsModel user = JsonSerializer.Deserialize<UserSocketsModel>(request.Argument.ToString());
            bool result = await userRepo.EditUserAsync(user);
            Request requestResponse = new Request
            {
                ActionType = ActionType.USER_EDIT.ToString(),
                Argument = JsonSerializer.Serialize(result)
            };
            if (actualRequest.Images != null && actualRequest.Images.Any())
            {
                if (user.Email != null)
                {
                    ImagesUtil.WriteImageToPath(actualRequest.Images[0], $"{FILE_PATH}/Users/{user.Id}", "/avatar.jpg");
                }
                else
                {
                    ImagesUtil.WriteImageToPath(actualRequest.Images[0], $"{FILE_PATH}/Users/{user.Id}",
                        "/background.jpg");
                }
            }

            return new ActualRequest
            {
                Request = requestResponse,
                Images = null
            };
        }
    }
}