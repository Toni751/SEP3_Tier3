using SEP3_Tier3.Repositories;
using SEP3_Tier3.Repositories.Implementation;

namespace SEP3_Tier3.Core
{
    public class RepositoriesFactory
    {
        private IAdminRepo adminRepo;
        private IUserRepo userRepo;

        public IAdminRepo AdminRepository()
        {
            if(adminRepo == null)
                adminRepo = new AdminRepo();
            return adminRepo;
        }

        public IUserRepo UserRepository()
        {
            if(userRepo == null)
                userRepo = new UserRepo();
            return userRepo;
        }
    }
}