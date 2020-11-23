using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using System.Threading;
using SEP3_Tier3.Core;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;
using SEP3_Tier3.SocketControllers;

namespace SEP3_Tier3
{
    public class ServerSocket
    {
        private SocketControllerFactory socketFactory;

        public ServerSocket(SocketControllerFactory socketFactory)
        {
            this.socketFactory = socketFactory;
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

            byte[] dataFromClient = new byte[65535];
            int bytesRead = stream.Read(dataFromClient, 0, dataFromClient.Length);
            Console.WriteLine("Bytes read length " + bytesRead);
            string readFromClientAsJson = Encoding.ASCII.GetString(dataFromClient, 0, bytesRead);
            Request readFromClient = JsonSerializer.Deserialize<Request>(readFromClientAsJson);
            Console.WriteLine("Request deserialized " + readFromClient.ActionType + readFromClient.Argument);
            
            ActualRequest actualRequest;
            if (readFromClient.ActionType.Equals(ActionType.HAS_IMAGES.ToString()))
            {
                List<int> incomingImageSizes = JsonSerializer.Deserialize<List<int>>(readFromClient.Argument.ToString());
                List<byte[]> incomingImages = new List<byte[]>();

                string confirmation = $"Waiting for {incomingImageSizes.Count} images";
                byte[] confirmationToClient = Encoding.ASCII.GetBytes(confirmation);
                stream.Write(confirmationToClient, 0, confirmationToClient.Length);

                foreach (var imageSize in incomingImageSizes)
                {
                    byte[] temp = new byte[imageSize];
                    byte[] finalImage = new byte[imageSize];
                    Console.WriteLine("Image size is " + imageSize);
                    int bytesLeftFromImage = imageSize;
                    int imageBytesRead;
                    do
                    {
                        imageBytesRead = stream.Read(temp, 0, bytesLeftFromImage);
                        Console.WriteLine("Read bytes " + imageBytesRead);
                        int difference = imageSize - bytesLeftFromImage;
                        Console.WriteLine("Difference is " + difference);
                        bytesLeftFromImage -= imageBytesRead;
                        Console.WriteLine("Bytes left from image " + bytesLeftFromImage);
                        for (int i = 0; i < imageBytesRead; i++)
                        {
                            finalImage[i + difference] = temp[i];
                        }
                    // i 3 3
                    // b 7 4
                    // d 0 3
                    // a 0..3  3..6
                    } while (bytesLeftFromImage > 0);
                    
                    Console.WriteLine("Image bytes read length " + imageBytesRead);
                    incomingImages.Add(finalImage);
                }

                dataFromClient = new byte[65535];
                bytesRead = stream.Read(dataFromClient, 0, dataFromClient.Length);
                Console.WriteLine("Bytes read length " + bytesRead);
                readFromClientAsJson = Encoding.ASCII.GetString(dataFromClient, 0, bytesRead);
                readFromClient = JsonSerializer.Deserialize<Request>(readFromClientAsJson);
                Console.WriteLine("Request deserialized " + readFromClient.ActionType + readFromClient.Argument);
                actualRequest = new ActualRequest
                {
                    Images = incomingImages,
                    Request = readFromClient
                };
            }
            else
            {
                actualRequest = new ActualRequest
                {
                    Images = null,
                    Request = readFromClient
                };
            }

            ActualRequest requestResponse;
            if (actualRequest.Request.ActionType.StartsWith("USER"))
                requestResponse = await socketFactory.UserSocketController().HandleClientRequest(actualRequest);
            else if (actualRequest.Request.ActionType.StartsWith("ADMIN"))
                requestResponse = await socketFactory.AdminSocketController().HandleClientRequest(actualRequest);
            else
                requestResponse = null;

            Request request = requestResponse.Request;
            List<byte[]> responseImages = requestResponse.Images;

            if (responseImages != null && responseImages.Any())
            {
                Console.WriteLine("Sending images " + responseImages.Count);
                List<int> imageSizes = new List<int>();
                
                foreach (var image in responseImages) {
                    imageSizes.Add(image.Length);
                }
                
                Request requestForImages = new Request{
                    ActionType = ActionType.HAS_IMAGES.ToString(), 
                    Argument = imageSizes
                };
                string requestForImagesAsJson = JsonSerializer.Serialize(requestForImages);
                Console.WriteLine("Sending back to client response: " + requestForImagesAsJson);
                byte[] dataForImages = Encoding.ASCII.GetBytes(requestForImagesAsJson);
                stream.Write(dataForImages, 0, dataForImages.Length);
                
                foreach (var image in responseImages) {
                   stream.Write(image, 0, image.Length);
                }
            }

            string requestResponseAsJson = JsonSerializer.Serialize(request);

            Console.WriteLine("Sending back to client response: " + requestResponseAsJson);
            byte[] dataToClient = Encoding.ASCII.GetBytes(requestResponseAsJson);
            stream.Write(dataToClient, 0, dataToClient.Length);

            client.Close();
        }
    }
}