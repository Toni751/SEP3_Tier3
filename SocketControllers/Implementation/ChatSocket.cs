using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;

namespace SEP3_Tier3.SocketControllers.Implementation
{
    public class ChatSocket : IChatSocket
    {
        private IChatRepo chatRepo;
        private readonly string FILE_PATH;

        public ChatSocket(IChatRepo chatRepo)
        {
            this.chatRepo = chatRepo;
            FILE_PATH = ImagesUtil.FILE_PATH;
        }

        public async Task<ActualRequest> HandleClientRequest(ActualRequest actualRequest)
        {
            switch (actualRequest.Request.ActionType)
            {
                case "MESSAGE_CREATE":
                    return await AddMessageAsync(actualRequest);
                case "MESSAGE_DELETE":
                    return await DeleteMessageAsync(actualRequest);
                case "MESSAGE_GET_LATEST":
                    return GetLastMessagesForUser(actualRequest);
                case "MESSAGE_GET_CONVERSATION":
                    return GetConversationForUsers(actualRequest);
                default:
                    return null;
            }
        }

        private async Task<ActualRequest> AddMessageAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            MessageSocketsModel message = JsonSerializer.Deserialize<MessageSocketsModel>(request.Argument.ToString());
            List<int> ints = await chatRepo.AddMessageAsync(message);
            int messageId = ints[0];
            Request requestResponse = new Request
            {
                ActionType = ActionType.MESSAGE_CREATE.ToString(),
                Argument = JsonSerializer.Serialize(messageId)
            };
            if (messageId > 0)
            {
                if(message.HasImage && actualRequest.Images != null)
                    try {
                        ImagesUtil.WriteImageToPath(actualRequest.Images[0], $"{FILE_PATH}/Messages", $"/{messageId}.jpg");
                    }
                    catch (Exception e) {
                        Console.WriteLine("Could not add image to created message " + messageId);
                    }
            }

            return new ActualRequest
            {
                Request = requestResponse,
                Images = null
            };
        }

        private async Task<ActualRequest> DeleteMessageAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int messageId = Convert.ToInt32(request.Argument.ToString());
            bool response = await chatRepo.DeleteMessageAsync(messageId);
            if (response)
                ImagesUtil.DeleteFile($"{FILE_PATH}/Messages", $"{messageId}.jpg");

            Request responseRequest = new Request
            {
                ActionType = ActionType.MESSAGE_DELETE.ToString(),
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        private ActualRequest GetLastMessagesForUser(ActualRequest actualRequest)
        {
            List<int> integers = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<UserShortVersionWithMessage> users = chatRepo.GetLastMessagesForUser(integers[0], integers[1]);
            Request response = new Request
            {
                ActionType = ActionType.MESSAGE_GET_LATEST.ToString(),
                Argument = JsonSerializer.Serialize(users)
            };
            List<byte[]> userAvatars = new List<byte[]>();
            if (users != null && users.Count > 0) {
                foreach (var user in users)
                {
                    if (user.Message.HasImage)
                        user.Message.Content = "<Image>";
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

        private ActualRequest GetConversationForUsers(ActualRequest actualRequest)
        {
            List<int> integers = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<MessageSocketsModel> messages = chatRepo.GetConversationForUsers(integers[0], integers[1], integers[2]);
            Request response = new Request
            {
                ActionType = ActionType.MESSAGE_GET_CONVERSATION.ToString(),
                Argument = JsonSerializer.Serialize(messages)
            };
            List<byte[]> messageImages = new List<byte[]>();
            if (messages != null && messages.Count > 0) {
                foreach (var message in messages)
                {
                    if (message.HasImage)
                        try {
                            var readImageFile = File.ReadAllBytes($"{FILE_PATH}/Messages/{message.Id}.jpg");
                            messageImages.Add(readImageFile);
                        }
                        catch (Exception e) {
                            Console.WriteLine("No picture found for message " + message.Id);
                        }
                }
            }
            return new ActualRequest
            {
                Request = response,
                Images = messageImages
            };
        }
    }
}