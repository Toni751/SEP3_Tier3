﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Internal;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;

namespace SEP3_Tier3.SocketControllers.Implementation
{
    /// <summary>
    /// Class for handling post-related sockets requests
    /// </summary>
    public class PostSocket : IPostSocket
    {
        private IPostRepo postRepo;
        private readonly string FILE_PATH;

        /// <summary>
        /// One-argument constructor initializing the post repository
        /// </summary>
        /// <param name="postRepo">the given value for the post repo</param>
        public PostSocket(IPostRepo postRepo)
        {
            this.postRepo = postRepo;
            FILE_PATH = ImagesUtil.FILE_PATH;
        }

        public async Task<ActualRequest> HandleClientRequest(ActualRequest actualRequest)
        {
            
            switch (actualRequest.Request.ActionType)
            {
                case "POST_CREATE":
                    return await AddPostAsync(actualRequest);
                case "POST_GET_BY_ID":
                    return await GetPostByIdAsync(actualRequest);
                case "POST_GET_FOR_USER":
                    return GetPostsForUser(actualRequest);
                case "POST_GET_BY_USER":
                    return GetPostsByUser(actualRequest);
                case "POST_EDIT":
                    return await UpdatePostAsync(actualRequest);
                case "POST_DELETE":
                    return await DeletePostAsync(actualRequest);
                case "POST_LIKE":
                case "POST_REPORT":
                    return await PostPostActionAsync(actualRequest);
                case "POST_ADD_COMMENT":
                    return await AddCommentToPostAsync(actualRequest);
                case "POST_DELETE_COMMENT":
                    return await DeleteCommentFromPostAsync(actualRequest);
                case "POST_GET_LIKES":
                    return GetAllLikesForPostAsync(actualRequest);
                case "POST_GET_COMMENTS":
                    return await GetAllCommentsForPostAsync(actualRequest);
                default:
                    return null;
            }
        }

        /// <summary>
        /// Persists a given post to the database
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> AddPostAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            PostShortVersion post = JsonSerializer.Deserialize<PostShortVersion>(request.Argument.ToString());
            post.HasImage = actualRequest.Images != null;
            Console.WriteLine("Post Sockets adding post " + post.Title);
            int result = await postRepo.AddPostAsync(post);
            Request responseRequest = new Request
            {
                ActionType = ActionType.POST_CREATE.ToString(),
                Argument = JsonSerializer.Serialize(result)
            };
            if (result > 0)
            {
                if (post.HasImage)
                    try {
                        ImagesUtil.WriteImageToPath(actualRequest.Images[0], $"{FILE_PATH}/Posts", $"/{result}.jpg");
                    }
                    catch (Exception e) {
                        Console.WriteLine("Could not add image to created post " + result);
                    }
            }
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        /// <summary>
        /// Retrieves a post with its owner, by id and with the sender status regarding it
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> GetPostByIdAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> ints = JsonSerializer.Deserialize<List<int>>(request.Argument.ToString());
            PostShortVersion post = await postRepo.GetPostByIdAsync(ints[0], ints[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.POST_GET_BY_ID.ToString(),
                Argument = JsonSerializer.Serialize(post)
            };
            List<byte[]> images = new List<byte[]>();

            if (post != null)
            {
                var readOwnerAvatar = File.ReadAllBytes($"{FILE_PATH}/Users/{post.Owner.UserId}/avatar.jpg");
                images.Add(ImagesUtil.ResizeImage(readOwnerAvatar, 20, 20));
                
                if (post.HasImage) {
                    var readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/Posts/{post.Id}.jpg");
                    images.Add(readAvatarFile);
                }
            }
            return new ActualRequest
            {
                Request = responseRequest,
                Images = images
            };
        }

        /// <summary>
        /// Returns a list with the ids of the latest posts for a user(i.e. made by him/friends/page he/she follows)
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest GetPostsForUser(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> ints = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<int> postIds = postRepo.GetLatestPostsForUserAsync(ints[0], ints[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.POST_GET_FOR_USER.ToString(),
                Argument = JsonSerializer.Serialize(postIds)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        /// <summary>
        /// Returns a list with the ids of the latest posts created by a user
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest GetPostsByUser(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            List<int> ints = JsonSerializer.Deserialize<List<int>>(actualRequest.Request.Argument.ToString());
            List<int> postIds = postRepo.GetLatestPostsByUser(ints[0], ints[1]);
            Request responseRequest = new Request
            {
                ActionType = ActionType.POST_GET_BY_USER.ToString(),
                Argument = JsonSerializer.Serialize(postIds)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
                //Images = images
            };
        }

        /// <summary>
        /// Edits a given post
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> UpdatePostAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            PostShortVersion updatedPost = JsonSerializer.Deserialize<PostShortVersion>(request.Argument.ToString());
            bool result = await postRepo.EditPostAsync(updatedPost);
            Request responseRequest = new Request
            {
                ActionType = ActionType.POST_EDIT.ToString(),
                Argument = JsonSerializer.Serialize(result)
            };
            if(!result)
                return new ActualRequest {
                    Request = responseRequest,
                    Images = null
                };
            
            if (actualRequest.Images != null && actualRequest.Images.Any())
            {
                ImagesUtil.WriteImageToPath(actualRequest.Images[0], $"{FILE_PATH}/Posts", $"/{updatedPost.Id}.jpg");
            }
            return new ActualRequest {
                Request = responseRequest,
                Images = null
            };
        }

        /// <summary>
        /// Deletes a post with a given id
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> DeletePostAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int postId = Convert.ToInt32(request.Argument.ToString());
            Console.WriteLine("Deleting post with id " + postId);
            bool response = await postRepo.DeletePostAsync(postId);
            if (response)
                ImagesUtil.DeleteFile($"{FILE_PATH}/Posts", $"{postId}.jpg");

            Request responseRequest = new Request
            {
                ActionType = ActionType.POST_DELETE.ToString(),
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        /// <summary>
        /// Persists a new post action to the database
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> PostPostActionAsync(ActualRequest actualRequest)
        {
            PostActionSockets postAction =
                JsonSerializer.Deserialize<PostActionSockets>(actualRequest.Request.Argument.ToString());
            Console.WriteLine("Posting post action " + postAction.ActionType);
            int notificationId = await postRepo.PostPostActionAsync(postAction);
            Request responseRequest = new Request
            {
                ActionType = postAction.ActionType,
                Argument = JsonSerializer.Serialize(notificationId)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        /// <summary>
        /// Adds a given comment to a given post
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> AddCommentToPostAsync(ActualRequest actualRequest)
        {
            CommentForPost comment =
                JsonSerializer.Deserialize<CommentForPost>(actualRequest.Request.Argument.ToString());
            int createdCommentId = await postRepo.AddCommentToPost(comment);
            Request responseRequest = new Request
            {
                ActionType = ActionType.POST_ADD_COMMENT.ToString(),
                Argument = JsonSerializer.Serialize(createdCommentId)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }

        /// <summary>
        /// Deletes a comment with a given id
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> DeleteCommentFromPostAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int commentId = Convert.ToInt32(request.Argument.ToString());
            Console.WriteLine("Deleting comment with id " + commentId);
            bool response = await postRepo.DeleteCommentFromPost(commentId);

            Request responseRequest = new Request
            {
                ActionType = ActionType.POST_DELETE_COMMENT.ToString(),
                Argument = JsonSerializer.Serialize(response)
            };
            return new ActualRequest
            {
                Request = responseRequest,
                Images = null
            };
        }
        
        /// <summary>
        /// Returns a list with all the users who reacted to a given post, in alphabetical order
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private ActualRequest GetAllLikesForPostAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int postId = Convert.ToInt32(request.Argument.ToString());
            List<UserShortVersion> users = postRepo.GetAllLikesForPost(postId);
            Request response = new Request
            {
                ActionType = ActionType.POST_GET_LIKES.ToString(),
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
        /// Returns a list with all the comments belonging to a given post, in reverse chronological order
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        private async Task<ActualRequest> GetAllCommentsForPostAsync(ActualRequest actualRequest)
        {
            Request request = actualRequest.Request;
            int postId = Convert.ToInt32(request.Argument.ToString());
            List<CommentSockets> comments = await postRepo.GetAllCommentsForPost(postId);
            Request response = new Request
            {
                ActionType = ActionType.POST_GET_COMMENTS.ToString(),
                Argument = JsonSerializer.Serialize(comments)
            };
            List<byte[]> userAvatars = new List<byte[]>();
            if (comments != null && comments.Count > 0)
            {
                foreach (var comment in comments) {
                    try {
                        var readAvatarFile = File.ReadAllBytes($"{FILE_PATH}/Users/{comment.Owner.UserId}/avatar.jpg");
                        userAvatars.Add(ImagesUtil.ResizeImage(readAvatarFile, 20, 20));
                    }
                    catch (Exception e) {
                        Console.WriteLine("No avatar found for user ");
                    }
                }
            }
            return new ActualRequest
            {
                Request = response,
                Images = userAvatars
            };
        }
    }
}