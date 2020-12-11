using System.Collections.Generic;
using SEP3_T3.Persistance;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories.UnitTestInterfaces
{
    /// <summary>
    /// Interface for the admin repository used for unit testing
    /// </summary>
    public interface IAdminRepoTest
    {
        /// <summary>
        /// The method for retrieving reported users for unit testing
        /// </summary>
        /// <param name="ctx">the database context</param>
        /// <param name="limit">the maximum number of users to be retrieved</param>
        /// <param name="offset">the number of users to be skipped when retrieving</param>
        /// <returns>the list of reported users order descending by the total number of reports</returns>
        List<UserShortVersion> GetAdminUsersWithDbContextAsync(ShapeAppDbContext ctx, int limit, int offset);
    }
}