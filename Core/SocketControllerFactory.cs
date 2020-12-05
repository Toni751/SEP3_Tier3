using System;
using SEP3_Tier3.SocketControllers;
using SEP3_Tier3.SocketControllers.Implementation;

namespace SEP3_Tier3.Core
{
    public class SocketControllerFactory
    {
        private RepositoriesFactory repositoriesFactory;
        private IAdminSocket adminSocket;
        private IUserSocket userSocket;
        private IPostSocket postSocket;
        private ITrainingSocket trainingSocket;

        public SocketControllerFactory(RepositoriesFactory repositoriesFactory)
        {
            this.repositoriesFactory = repositoriesFactory;
        }

        public IAdminSocket AdminSocket {
            get {
                if (adminSocket == null)
                    adminSocket = new AdminSocket(repositoriesFactory.AdminRepo);
                return adminSocket;
            }
        }

        public IUserSocket UserSocket {
            get {
                if (userSocket == null)
                    userSocket = new UserSocket(repositoriesFactory.UserRepo);
                return userSocket;
            }
        }
        
        public IPostSocket PostSocket {
            get {
                if (postSocket == null)
                    postSocket = new PostSocket(repositoriesFactory.PostRepo);
                return postSocket;
            }
        }

        public ITrainingSocket TrainingSocket
        {
            get
            {
                if(trainingSocket == null)
                    trainingSocket = new TrainingSocket(repositoriesFactory.TrainingRepo);
                return trainingSocket;
            }
        }
    }
}