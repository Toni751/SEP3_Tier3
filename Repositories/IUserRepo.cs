using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    /// <summary>
    /// Interface storing the functionality of the user repository class
    /// </summary>
    public interface IUserRepo
    {
        /// <summary>
        /// Persists a given user to the database
        /// </summary>
        /// <param name="user">the user to be added</param>
        /// <returns>the id of the created user</returns>
        Task<int> AddUserAsync(UserSocketsModel user);
        
        /// <summary>
        /// Retrieves the user with the given email and password, if any
        /// </summary>
        /// <param name="email">the given email</param>
        /// <param name="password">the given password</param>
        /// <returns>the user with the given email and password, if any</returns>
        Task<UserShortVersion> LoginAsync(string email, string password);
        
        /// <summary>
        /// Retrieves a user by id, and his status regarding the sender
        /// </summary>
        /// <param name="senderId">the id of the user who sent the request</param>
        /// <param name="receiverId">the id of the user targeted by the request</param>
        /// <returns>the user with the given id</returns>
        Task<UserSocketsModel> GetUserByIdAsync(int senderId, int receiverId);
        
        /// <summary>
        /// Edits a given user
        /// </summary>
        /// <param name="user">the new value for the user</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> EditUserAsync(UserSocketsModel user);
        
        /// <summary>
        /// Deletes a user with a given id
        /// </summary>
        /// <param name="userId">the id of the user</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> DeleteUserAsync(int userId);
        
        /// <summary>
        /// Creates a new user action and its corresponding notification, if any
        /// </summary>
        /// <param name="modelActionSockets">the user action to be added</param>
        /// <returns>the id of the created notification, if any, 0 otherwise</returns>
        Task<int> PostUserActionAsync(ModelActionSockets modelActionSockets);
        
        /// <summary>
        /// Deletes a friendship between 2 given users, stored in the user action
        /// </summary>
        /// <param name="modelActionSockets">the user action</param>
        /// <returns>0 if the action was successful, -1 otherwise</returns>
        Task<int> RemoveFriendshipAsync(ModelActionSockets modelActionSockets);
        
        /// <summary>
        /// Adds a rating from a given user to a given page, stored in the user action
        /// </summary>
        /// <param name="modelActionSockets">the user action</param>
        /// <returns>0 if the action was successful, -1 otherwise</returns>
        Task<int> PostPageRatingAsync(ModelActionSockets modelActionSockets);
        
        /// <summary>
        /// Deletes a notification with the given id
        /// </summary>
        /// <param name="notificationId">the id of the notification</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> DeleteNotificationAsync(int notificationId);
        
        /// <summary>
        /// Retrieves the ids of the posts belonging to a given user
        /// </summary>
        /// <param name="userId">the given user</param>
        /// <returns>a list with the ids of the posts belonging to a given user</returns>
        List<int> GetPostIdsForUser(int userId);
        
        /// <summary>
        /// Retrieves the users whose username starts with the given filter string, if any
        /// </summary>
        /// <param name="filterString">the given filter string</param>
        /// <returns>the users whose username starts with the given filter string, if any</returns>
        List<SearchBarUser> GetUsersByFilter(string filterString);
        
        /// <summary>
        /// Returns a list with all the gyms in a given city
        /// </summary>
        /// <param name="city">the name of the city</param>
        /// <returns>a list with all the gyms in a given city</returns>
        List<UserShortVersion> GetAllGymsInCity(string city);
        
        /// <summary>
        /// Gets a list with all the notifications belonging to a given user
        /// </summary>
        /// <param name="userId">the id of the user</param>
        /// <returns>a list with all the notifications belonging to a given user</returns>
        List<NotificationSockets> GetNotificationsForUser(int userId);
        
        /// <summary>
        /// Retrieves a list of the target user's friends, or common friends with the sender
        /// </summary>
        /// <param name="userId">the target user id</param>
        /// <param name="senderId">the id of the user who sent the request</param>
        /// <param name="offset">the number of friends to be skipped</param>
        /// <returns>a list of the target user's friends, or common friends with the sender</returns>
        List<UserShortVersionWithStatus> GetFriendsForUser(int userId, int senderId, int offset);
        
        /// <summary>
        /// Increments a given user's score by a given amount
        /// </summary>
        /// <param name="userId">the given user id</param>
        /// <param name="amount">the given amount</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> IncrementUserScoreAsync(int userId, int amount);
        
        /// <summary>
        /// Gets a list with all the online friends belonging to a given user
        /// </summary>
        /// <param name="userId">the id of the user</param>
        /// <returns>a list with all the online friends belonging to a given user</returns>
        List<UserShortVersion> GetOnlineFriendsForUser(int userId);
        
        /// <summary>
        /// Logs in our out a user and returns a list with his online friends ids, so that they can be notified
        /// </summary>
        /// <param name="userId">the id of the user</param>
        /// <param name="isLogout">true if the action is logout, false otherwise</param>
        /// <returns>a list with the user's online friends ids</returns>
        List<int> LogoutOrInUser(int userId, bool isLogout);
        
        /// <summary>
        /// Retrieves a user short version instance with the given user id
        /// </summary>
        /// <param name="userId">the id of the user</param>
        /// <returns>a user short version instance with the given user id</returns>
        UserShortVersion GetUserShortVersionById(int userId);
    }
}