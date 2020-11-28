using System.Threading.Tasks;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.SocketControllers
{
    public interface IPostSocket
    {
        Task<ActualRequest> HandleClientRequest(ActualRequest actualRequest);
    }
}