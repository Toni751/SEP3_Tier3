using System;
using System.Collections.Generic;
using System.IO;
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
        private const string FILE_PATH = "C:/Users/Toni/RiderProjects/Images";

        public UserSocket(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
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
                default:
                    return null;
            }
        }

        private async Task<ActualRequest> AddUserAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            string userAsJson = request.Argument.ToString();
            UserSocketsModel user = JsonSerializer.Deserialize<UserSocketsModel>(userAsJson);
            Console.WriteLine("Server got register user " + JsonSerializer.Serialize(user));
            bool result = await userRepo.AddUserAsync(user);
            Request requestResponse = new Request
            {
                ActionType = ActionType.USER_REGISTER.ToString(),
                Argument = JsonSerializer.Serialize(result)
            };
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
                Argument = loginResult
            };
            byte[] readFile;
            List<byte[]> images = new List<byte[]>();
            try
            {
                readFile = File.ReadAllBytes($"{FILE_PATH}/Avatar/{loginResult.UserId}.png");
                images.Add(readFile);
            }
            catch (Exception e)
            {
                Console.WriteLine("No avatar found for user " + loginResult.UserId);
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
            int userId = Convert.ToInt32(request.Argument.ToString());
            Console.WriteLine("Retrieving user with id " + userId);
            User user = await userRepo.GetUserByIdAsync(userId);
            Request requestResponse = new Request
            {
                ActionType = ActionType.USER_GET_BY_ID.ToString(),
                Argument = JsonSerializer.Serialize(user)
            };
            byte[] readAvatarFile;
            byte[] readBackgroundFile;
            List<byte[]> images = new List<byte[]>();
            try
            {
                // /Images/users/{userId}/....
                // /Images/posts/{postId}/....
                readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/Avatar/{user.Id}.png");
                images.Add(ResizeImage(readAvatarFile, 200, 200));
                readBackgroundFile = File.ReadAllBytes($"{FILE_PATH}/Background/{user.Id}.jpg");
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
            User user = (User) request.Argument;
            Request requestResponse = new Request
            {
                ActionType = ActionType.USER_EDIT.ToString(),
                Argument = JsonSerializer.Serialize(user)
            };
            byte[] readAvatarFile;
            byte[] readBackgroundFile;
            List<byte[]> images = new List<byte[]>();
            try
            {
                // /Images/users/{userId}/....
                // /Images/posts/{postId}/....
                readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/{user.Id}/Avatar.png");
                images.Add(readAvatarFile);
                readBackgroundFile = File.ReadAllBytes($"{FILE_PATH}/{user.Id}/Background.jpg");
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

        private byte[] ResizeImage(byte[] initialImage, int width, int height)
        {
            SKBitmap source = SKBitmap.Decode(initialImage);
            using SKBitmap scaledBitmap = source.Resize(new SKImageInfo(width, height), SKFilterQuality.Medium);
            using SKImage scaledImage = SKImage.FromBitmap(scaledBitmap);
            using SKData data = scaledImage.Encode();
            return data.ToArray();
        }
    }
}