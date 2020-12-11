using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    /// <summary>
    /// Interface storing the functionality of the chat repository class
    /// </summary>
    public interface IChatRepo
    {
        /// <summary>
        /// Persists a given message to the database and its corresponding notification
        /// </summary>
        /// <param name="message">the message to be added</param>
        /// <returns>the ids of the created message and notification</returns>
        Task<List<int>> AddMessageAsync(MessageSocketsModel message);
        
        /// <summary>
        /// Deletes a message with a given id by setting its content to null and deleting its picture
        /// </summary>
        /// <param name="messageId">the id of the message</param>
        /// <returns>true if the action is successful, false otherwise</returns>
        Task<bool> DeleteMessageAsync(int messageId);
        
        /// <summary>
        /// Retrieves the last messages for a user with his friends/following gyms for unit testing
        /// </summary>
        /// <param name="userId">the id of the user</param>
        /// <param name="offset">the number of messages to be skipped</param>
        /// <returns>A list with a user's last messages with friends</returns>
        List<UserShortVersionWithMessage> GetLastMessagesForUser(int userId, int offset);
        
        /// <summary>
        /// Returns a list with recent messages between 2 given users
        /// </summary>
        /// <param name="firstUserId">the id of the first user</param>
        /// <param name="secondUserId">the id of the second user</param>
        /// <param name="offset">the number of messages to be skipped</param>
        /// <returns>A list of recent messages between the 2 users</returns>
        List<MessageSocketsModel> GetConversationForUsers(int firstUserId, int secondUserId, int offset);
    }
}