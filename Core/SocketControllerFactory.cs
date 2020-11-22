using SEP3_Tier3.SocketControllers;
using SEP3_Tier3.SocketControllers.Implementation;

namespace SEP3_Tier3.Core
{
    public class SocketControllerFactory
    {
        private RepositoriesFactory repositoriesFactory;
        private IAdminSocket adminSocket;
        private IUserSocket userSocket;

        public SocketControllerFactory(RepositoriesFactory repositoriesFactory)
        {
            this.repositoriesFactory = repositoriesFactory;
        }

        public IAdminSocket AdminSocketController()
        {
            if (adminSocket == null)
                adminSocket = new AdminSocket(repositoriesFactory.AdminRepository());
            return adminSocket;
        }

        public IUserSocket UserSocketController()
        {
            if(userSocket == null)
                userSocket = new UserSocket(repositoriesFactory.UserRepository());
            return userSocket;
        }
    }
}