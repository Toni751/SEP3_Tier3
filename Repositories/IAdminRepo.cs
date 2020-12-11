using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories
{
    /// <summary>
    /// Interface storing the functionality of the admin repository class
    /// </summary>
    public interface IAdminRepo
    {
        /// <summary>
        /// Gets a list of the most reported users
        /// </summary>
        /// <param name="limit">the maximum number of users to be retrieved</param>
        /// <param name="offset">the number of users to be skipped when retrieving</param>
        /// <returns>the list of the users with the most reports</returns>
        Task<List<UserShortVersion>> GetAdminUsersAsync(int limit, int offset);
        
        /// <summary>
        /// Gets a list of the most reported posts
        /// </summary>
        /// <param name="limit">the maximum number of posts to be retrieved</param>
        /// <param name="offset">the number of posts to be skipped when retrieving</param>
        /// <returns>the list of the posts with the most reports</returns>
        Task<List<int>> GetAdminPostsAsync(int limit, int offset);
        
        /// <summary>
        /// Returns the total number of entries of a given model
        /// </summary>
        /// <param name="modelType">the model type, can be either users or posts</param>
        /// <returns>the total number of entries of the given model type</returns>
        Task<int> GetTotalNumberAsync(string modelType);
    }
}