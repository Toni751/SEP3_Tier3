using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    public interface IChatRepo
    {
        Task<List<int>> AddMessageAsync(MessageSocketsModel message);
        Task<bool> DeleteMessageAsync(int messageId);
        List<UserShortVersionWithMessage> GetLastMessagesForUser(int userId, int offset);
        List<MessageSocketsModel> GetConversationForUsers(int firstUserId, int secondUserId, int offset);
    }
}