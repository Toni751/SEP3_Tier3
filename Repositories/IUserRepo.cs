using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    public interface IUserRepo
    {
        Task<bool> AddUserAsync(UserSocketsModel user);
        Task<UserShortVersion> LoginAsync(string email, string password);
        Task<List<Post>> GetLatestPostsForUserAsync(int id, int offset);
        Task<User> GetUserByIdAsync(int id);
    }
}