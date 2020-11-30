using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;

namespace SEP3_Tier3.SocketControllers.Implementation
{
    public class AdminSocket : IAdminSocket
    {
        private IAdminRepo adminRepo;
        private readonly string FILE_PATH;

        public AdminSocket(IAdminRepo adminRepo)
        {
            this.adminRepo = adminRepo;
            FILE_PATH = ImagesUtil.FILE_PATH;
        }

        public async Task<ActualRequest> HandleClientRequest(ActualRequest actualRequest)
        {
            switch (actualRequest.Request.ActionType)
            {
                case "ADMIN_GET_USERS":
                    return await GetUsers(actualRequest);
                case "ADMIN_GET_POSTS":
                    return await GetPosts(actualRequest);
                case "ADMIN_GET_NUMBER":
                    return await GetNumber(actualRequest);
                default:
                    return null;
            }
        }

        private async Task<ActualRequest> GetNumber(ActualRequest actualRequest)
        {
            string modelType = actualRequest.Request.Argument.ToString();
            int totalNumberOfModel = await adminRepo.GetTotalNumberAsync(modelType);
            Request request = new Request
            {
                ActionType = ActionType.ADMIN_GET_NUMBER.ToString(),
                Argument = JsonSerializer.Serialize(totalNumberOfModel)
            };
            return new ActualRequest
            {
                Request = request,
                Images = null
            };
        }

        private async Task<ActualRequest> GetUsers(ActualRequest actualRequest)
        {
            List<int> paginationInts = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<UserShortVersion> users = await adminRepo.GetAdminUsersAsync(paginationInts[0], paginationInts[1]);
            Request request = new Request
            {
                ActionType = ActionType.ADMIN_GET_USERS.ToString(),
                Argument = JsonSerializer.Serialize(users)
            };
            List<byte[]> userAvatars = new List<byte[]>();
            if (users != null && users.Count > 0) {
                foreach (var user in users) {
                    try {
                        var readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/Users/{user.UserId}/avatar.jpg");
                        userAvatars.Add(ImagesUtil.ResizeImage(readAvatarFile, 50, 50));
                    }
                    catch (Exception e) {
                        Console.WriteLine("No avatar found for user " + user.UserId);
                    }
                }
            }
            return new ActualRequest
            {
                Request = request,
                Images = userAvatars
            };
        }

        private async Task<ActualRequest> GetPosts(ActualRequest actualRequest)
        {
            List<int> paginationInts = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<int> postIds = await adminRepo.GetAdminPostsAsync(paginationInts[0], paginationInts[1]);
            Request request = new Request
            {
                ActionType = ActionType.ADMIN_GET_POSTS.ToString(),
                Argument = JsonSerializer.Serialize(postIds)
            };
            // List<byte[]> postImages = new List<byte[]>();
            // if (posts != null && posts.Count > 0) {
            //     foreach (var post in posts) {
            //         try {
            //             var readImageFile = File.ReadAllBytes($"{FILE_PATH}/Posts/{post.Id}.jpg");
            //             postImages.Add(ImagesUtil.ResizeImage(readImageFile, 50, 50));
            //         }
            //         catch (Exception e) {
            //             Console.WriteLine("No image found for post " + post.Id);
            //         }
            //     }
            // }

            return new ActualRequest
            {
                Request = request,
                Images = null
            };
        }
    }
}