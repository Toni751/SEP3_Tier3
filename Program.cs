using System;
using SEP3_Tier3.Repositories;
using SEP3_Tier3.Repositories.Implementation;
using SEP3_Tier3.SocketControllers;
using SEP3_Tier3.SocketControllers.Implementation;

namespace SEP3_Tier3
{
    class Program
    {
        static void Main(string[] args)
        {
            IUserRepo userRepo = new UserRepo();
            IUserSocket userSocket = new UserSocket(userRepo);
            ServerSocket serverSocket = new ServerSocket(userSocket);
            serverSocket.Start();
        }
    }
}