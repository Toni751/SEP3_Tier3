﻿using System;
using SEP3_Tier3.Repositories;
using SEP3_Tier3.Repositories.Implementation;

namespace SEP3_Tier3.Core
{
    /// <summary>
    /// Class for managing all the repositories
    /// </summary>
    public class RepositoriesFactory
    {
        private IAdminRepo adminRepo;
        private IUserRepo userRepo;
        private IPostRepo postRepo;
        private ITrainingRepo trainingRepo;
        private IDietRepo dietRepo;
        private IChatRepo chatRepo;
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
                return postRepo;
            }   
        }

        public ITrainingRepo TrainingRepo {
            get {
                if(trainingRepo == null)
                    trainingRepo = new TrainingRepo();
                return trainingRepo;
            }
        }

        public IDietRepo DietRepo {
            get {
                if(dietRepo == null)
                    dietRepo = new DietRepo();
                return dietRepo;
            }
        }

        public IChatRepo ChatRepo {
            get {
                if(chatRepo == null)
                    chatRepo = new ChatRepo();
                return chatRepo;
            }
        }
    }
}