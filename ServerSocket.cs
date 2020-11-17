using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;
using SEP3_Tier3.SocketControllers;

namespace SEP3_Tier3
{
    public class ServerSocket
    {
        private IUserSocket userSocket;
        public ServerSocket(IUserSocket userSocket)
        {
            this.userSocket = userSocket;
        }

        public void Start()
        {
            Console.WriteLine("Starting server..");

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            TcpListener listener = new TcpListener(ip, 2910);
            listener.Start();

            Console.WriteLine("Server started..");

            while (true)
            {
                TcpClient client = listener.AcceptTcpClient();

                Console.WriteLine("Client connected");
                new Thread(() => HandleClientRequest(client)).Start();
            }
        }

        private async void HandleClientRequest(TcpClient client)
        {
            NetworkStream stream = client.GetStream();

            byte[] dataFromClient = new byte[1024];
            int bytesRead = stream.Read(dataFromClient, 0, dataFromClient.Length);
            Console.WriteLine("Bytes read length " + bytesRead);
            string readFromClientAsJson = Encoding.ASCII.GetString(dataFromClient, 0, bytesRead);
            Request readFromClient = JsonSerializer.Deserialize<Request>(readFromClientAsJson);
            Console.WriteLine("Request deserialized " + readFromClient.ActionType + readFromClient.Argument);

            Request requestResponse;
            if (readFromClient.ActionType.ToString().Contains("USER"))
                requestResponse = await userSocket.HandleClientRequest(readFromClient);
            else
                requestResponse = null;
            // string receivedUserAsJson = readFromClient.Argument.ToString();
            // Console.WriteLine("User as json " + receivedUserAsJson);
            // User deserializedUserFromClient = JsonSerializer.Deserialize<User>(receivedUserAsJson);
            // Console.WriteLine("Des user " + deserializedUserFromClient);

            // if (readFromClient.GetType().ToString().Equals("exit"))
            //     break;

            // Message sendToClient = new Message
            // {
            //     Id = readFromClient.Id,
            //     Content = readFromClient.Content.ToUpper()
            // };
            //Thread.Sleep(3000);
            // user.Username += count++;
            // Request request1 = new Request
            // {
            //     ActionType = ActionType.SEND_MESSAGE.ToString(),
            //     Argument = JsonSerializer.Serialize(user)
            // };

            string requestResponseAsJson = JsonSerializer.Serialize(requestResponse);
            Console.WriteLine("Sending back to client response: " + requestResponseAsJson);
            byte[] dataToClient = Encoding.ASCII.GetBytes(requestResponseAsJson);
            stream.Write(dataToClient, 0, dataToClient.Length);

            client.Close();
        }
    }
}