using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    public interface IAdminRepo
    {
        Task<List<UserShortVersion>> GetAdminUsersAsync(int limit, int offset);
        Task<List<PostShortVersion>> GetAdminPostsAsync(int limit, int offset);
    }
}