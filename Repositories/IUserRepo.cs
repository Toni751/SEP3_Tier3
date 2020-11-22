using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    public interface IUserRepo
    {
        Task<int> AddUserAsync(UserSocketsModel user);
        Task<UserShortVersion> LoginAsync(string email, string password);
        Task<List<Post>> GetLatestPostsForUserAsync(int id, int offset);
        Task<UserSocketsModel> GetUserByIdAsync(int senderId, int receiverId);
        Task<bool> EditUserAsync(UserSocketsModel user);
        Task<bool> DeleteUserAsync(int userId);
        Task<bool> PostUserActionAsync(UserActionSockets userActionSockets);
        Task<bool> RemoveFriendshipAsync(UserActionSockets userActionSockets);
        Task<bool> PostPageRatingAsync(UserActionSockets userActionSockets);
    }
}