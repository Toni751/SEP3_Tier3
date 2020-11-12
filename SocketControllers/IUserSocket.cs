using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_T3.Models;

namespace SEP3_T3.SocketControllers
{
    public interface IUserSocket
    {
        Task AddUserAsync(RegularUser user);
        Task<UserShortVersion> LoginAsync(string email, string password);
        Task<List<Post>> GetLatestPostsForUserAsync(int id, int offset);
        Task<RegularUser> GetUserByIdAsync(int id);
    }
}