using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    /// <summary>
    /// Interface storing the functionality of the post repository class
    /// </summary>
    public interface IPostRepo
    {
        /// <summary>
        /// Persists a given post to the database
        /// </summary>
        /// <param name="post">the post to be added</param>
        /// <returns>the id of the created post</returns>
        Task<int> AddPostAsync(PostShortVersion post);
        
        /// <summary>
        /// Retrieves a post with its owner, by id and with the sender status regarding it
        /// </summary>
        /// <param name="postId">the id of the post</param>
        /// <param name="userId">the id of the user making the request</param>
        /// <returns>the post with its owner</returns>
        Task<PostShortVersion> GetPostByIdAsync(int postId, int userId);
        
        /// <summary>
        /// Edits a given post
        /// </summary>
        /// <param name="post">the new value for the post</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> EditPostAsync(PostShortVersion post);
        
        /// <summary>
        /// Deletes a post with a given id
        /// </summary>
        /// <param name="postId">the id of the post</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> DeletePostAsync(int postId);
        
        /// <summary>
        /// Returns a list with the ids of the latest posts for a user(i.e. made by him/friends/page he/she follows)
        /// </summary>
        /// <param name="userId">the id of the user</param>
        /// <param name="offset">the number of posts to be skipped</param>
        /// <returns>a list with the ids of the latest posts for a user</returns>
        List<int> GetLatestPostsForUserAsync(int userId, int offset);
        
        /// <summary>
        /// Returns a list with the ids of the latest posts created by a user
        /// </summary>
        /// <param name="userId">the id of the user</param>
        /// <param name="offset">the number of posts to be skipped</param>
        /// <returns>a list with the ids of the latest posts created by the user</returns>
        List<int> GetLatestPostsByUser(int userId, int offset);
        
        /// <summary>
        /// Persists a new post action to the database
        /// </summary>
        /// <param name="postActionSockets">the post action to be added</param>
        /// <returns>the id of the added post action</returns>
        Task<int> PostPostActionAsync(PostActionSockets postActionSockets);
        
        /// <summary>
        /// Adds a given comment to a given post
        /// </summary>
        /// <param name="comment">the comment to be added, with the post it belongs to</param>
        /// <returns>the id of the new comment</returns>
        Task<int> AddCommentToPost(CommentForPost comment);
        
        /// <summary>
        /// Deletes a comment with a given id
        /// </summary>
        /// <param name="commentId">the id of the comment</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> DeleteCommentFromPost(int commentId);
        
        /// <summary>
        /// Returns a list with all the comments belonging to a given post, in reverse chronological order
        /// </summary>
        /// <param name="postId">the id of the post</param>
        /// <returns>a list with all the comments belonging to a given post</returns>
        Task<List<CommentSockets>> GetAllCommentsForPost(int postId);
        
        /// <summary>
        /// Returns a list with all the users who reacted to a given post, in alphabetical order
        /// </summary>
        /// <param name="postId">the id of the post</param>
        /// <returns>a list with all the users who reacted to a given post</returns>
        List<UserShortVersion> GetAllLikesForPost(int postId);
    }
}