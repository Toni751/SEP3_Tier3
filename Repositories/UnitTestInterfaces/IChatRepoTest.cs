using System.Collections.Generic;
using SEP3_T3.Persistance;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories.UnitTestInterfaces
{
    /// <summary>
    /// Interface for the chat repository used for unit testing
    /// </summary>
    public interface IChatRepoTest
    {
        /// <summary>
        /// Retrieves the last messages for a user with his friends/following gyms for unit testing
        /// </summary>
        /// <param name="ctx">the database context to be used</param>
        /// <param name="userId">the id of the user</param>
        /// <param name="offset">the number of messages to be skipped</param>
        /// <returns>A list with a user's last messages with friends</returns>
        List<UserShortVersionWithMessage> GetLastMessagesForUserWithDbContext(ShapeAppDbContext ctx, int userId, int offset);
    }
}