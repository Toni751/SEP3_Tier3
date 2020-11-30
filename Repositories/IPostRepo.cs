using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    public interface IPostRepo
    {
        Task<int> AddPostAsync(PostShortVersion post);
        Task<PostShortVersion> GetPostByIdAsync(int postId, int userId);
        Task<bool> EditPostAsync(PostShortVersion post);
        Task<bool> DeletePostAsync(int postId);
        List<int> GetLatestPostsForUserAsync(int userId, int offset);
        List<int> GetLatestPostsByUser(int userId, int offset);
        Task<int> PostPostActionAsync(PostActionSockets postActionSockets);
        Task<int> AddCommentToPost(CommentForPost comment);
        Task<bool> DeleteCommentFromPost(int commentId);
        Task<List<CommentSockets>> GetAllCommentsForPost(int postId);
        Task<List<UserShortVersion>> GetAllLikesForPost(int postId);
    }
}