using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;


namespace SEP3_Tier3.SocketControllers
{
    public interface IUserSocket
    {
        Task<ActualRequest> HandleClientRequest(ActualRequest actualRequest);
        // Task<Request> AddUserAsync(Request request);
        // Task<Request> LoginAsync(Request request);
        // Task<List<Post>> GetLatestPostsForUserAsync(Request request);
        // Task<User> GetUserByIdAsync(Request request);
    }
}