using SEP3_Tier3.Repositories;
using SEP3_Tier3.Repositories.Implementation;

namespace SEP3_Tier3.Core
{
    public class RepositoriesFactory
    {
        private IAdminRepo adminRepo;
        private IUserRepo userRepo;
        private IPostRepo postRepo;

        public IAdminRepo AdminRepo {
            get {
                if(adminRepo == null)
                    adminRepo = new AdminRepo();
                return adminRepo;
            }
        }
        
        public IUserRepo UserRepo {
            get {
                if(userRepo == null)
                   userRepo = new UserRepo();
                return userRepo;
            }
        }

        public IPostRepo PostRepo {
            get {
                if(postRepo == null)
                    postRepo = new PostRepo();
                return PostRepo;
            }   
        }

        // public IAdminRepo AdminRepository()
        // {
        //     if(adminRepo == null)
        //         adminRepo = new AdminRepo();
        //     return adminRepo;
        // }
        //
        // public IUserRepo UserRepository()
        // {
        //     if(userRepo == null)
        //         userRepo = new UserRepo();
        //     return userRepo;
        // }
    }
}