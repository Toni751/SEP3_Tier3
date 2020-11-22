using System;
using SEP3_Tier3.Core;
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
            RepositoriesFactory repoFactory = new RepositoriesFactory();
            SocketControllerFactory socketFactory = new SocketControllerFactory(repoFactory);
            ServerSocket serverSocket = new ServerSocket(socketFactory);
            serverSocket.Start();
        }
    }
}