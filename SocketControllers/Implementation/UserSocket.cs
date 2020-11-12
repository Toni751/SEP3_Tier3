using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_T3.Models;

namespace SEP3_T3.SocketControllers.Implementation
{
    public class UserSocket : IUserSocket
    {
        public Task AddUserAsync(RegularUser user)
        {
            throw new System.NotImplementedException();
        }

        public Task<UserShortVersion> LoginAsync(string email, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<Post>> GetLatestPostsForUserAsync(int id, int offset)
        {
            throw new System.NotImplementedException();
        }

        public Task<RegularUser> GetUserByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}