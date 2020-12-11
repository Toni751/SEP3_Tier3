using System.Collections.Generic;
using System.Threading.Tasks;
using SEP3_Tier3.Models;


namespace SEP3_Tier3.SocketControllers
{
    /// <summary>
    /// Interface storing the main functionality of the user sockets controller class
    /// </summary>
    public interface IUserSocket
    {
        /// <summary>
        /// Method for handling client requests and providing a response
        /// </summary>
        /// <param name="actualRequest">the client request to be handled</param>
        /// <returns>the response to the given request</returns>
        Task<ActualRequest> HandleClientRequest(ActualRequest actualRequest);
    }
}