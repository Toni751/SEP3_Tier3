using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_T3.Models;

namespace SEP3_T3.Repositories.Implementation
{
    public class UserRepo : IUserRepo
    {
        public async Task AddUserAsync(RegularUser user)
        {
            throw new System.NotImplementedException();
        }

        public async Task<UserShortVersion> LoginAsync(string email, string password)
        {
            throw new System.NotImplementedException();
        }

        public async Task<List<Post>> GetLatestPostsForUserAsync(int id, int offset)
        {
            throw new System.NotImplementedException();
        }

        public async Task<RegularUser> GetUserByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }
    }
}