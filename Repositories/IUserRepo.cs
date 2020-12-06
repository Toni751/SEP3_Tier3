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
        Task<UserSocketsModel> GetUserByIdAsync(int senderId, int receiverId);
        Task<bool> EditUserAsync(UserSocketsModel user);
        Task<bool> DeleteUserAsync(int userId);
        Task<int> PostUserActionAsync(ModelActionSockets modelActionSockets);
        Task<int> RemoveFriendshipAsync(ModelActionSockets modelActionSockets);
        Task<int> PostPageRatingAsync(ModelActionSockets modelActionSockets);
        Task<bool> DeleteNotificationAsync(int notificationId);
        List<int> GetPostIdsForUser(int userId);
        List<SearchBarUser> GetUsersByFilter(string filterString);
        List<UserShortVersion> GetAllGymsInCity(string city);
        List<NotificationSockets> GetNotificationsForUser(int userId);
        List<UserShortVersionWithStatus> GetFriendsForUser(int userId, int senderId, int offset);
    }
}