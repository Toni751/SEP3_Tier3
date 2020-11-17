using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories;


namespace SEP3_Tier3.SocketControllers.Implementation
{
    public class UserSocket : IUserSocket
    {
        private IUserRepo userRepo;

        public UserSocket(IUserRepo userRepo)
        {
            this.userRepo = userRepo;
        }

        public async Task<Request> HandleClientRequest(Request request)
        {
            switch (request.ActionType)
            {
                case "USER_REGISTER":
                    return await AddUserAsync(request);
                case "USER_LOGIN":
                    return await LoginAsync(request);
                default:
                    return null;
            }
        }

        public async Task<Request> AddUserAsync(Request request)
        {
            string userAsJson = request.Argument.ToString();
            UserSocketsModel user = JsonSerializer.Deserialize<UserSocketsModel>(userAsJson);
            Console.WriteLine("Server got register user " + JsonSerializer.Serialize(user));
            bool result = await userRepo.AddUserAsync(user);
            return new Request
            {
                ActionType = ActionType.USER_REGISTER.ToString(),
                Argument = JsonSerializer.Serialize(result)
            };
        }

        public async Task<Request> LoginAsync(Request request)
        {
            string credentialsAsJson = request.Argument.ToString();
            LoginCredentials loginCredentials = JsonSerializer.Deserialize<LoginCredentials>(credentialsAsJson);
            Console.WriteLine("Got login credentials " + loginCredentials.Email + loginCredentials.Password);
            UserShortVersion loginResult = await userRepo.LoginAsync(loginCredentials.Email, loginCredentials.Password);
            return new Request {
                ActionType = ActionType.USER_LOGIN.ToString(),
                Argument = loginResult
            };
        }

        public Task<List<Post>> GetLatestPostsForUserAsync(Request request)
        {
            throw new System.NotImplementedException();
        }

        public Task<User> GetUserByIdAsync(Request request)
        {
            throw new System.NotImplementedException();
        }
    }
}