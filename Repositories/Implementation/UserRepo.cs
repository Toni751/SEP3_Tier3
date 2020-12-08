using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SEP3_T3.Persistance;
using SEP3_Tier3.Migrations;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories.Implementation
{
    public class UserRepo : IUserRepo
    {
        private List<int> onlineUserIds;

        public UserRepo()
        {
            onlineUserIds = new List<int>();
        }

        public async Task<int> AddUserAsync(UserSocketsModel user)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    await ctx.Users.AddAsync(new User
                    {
                        Address = user.Address,
                        City = user.City,
                        Description = user.Description,
                        Email = user.Email,
                        Name = user.Name,
                        Password = user.Password
                    });
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return -1;
                }

                int userId = ctx.Users.First(u => u.Email.Equals(user.Email)).Id;
                Console.WriteLine("Added user to database with id " + userId);
                return userId;
            }
        }

        public async Task<UserShortVersion> LoginAsync(string email, string password)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                User user = await ctx.Users.Where(u => u.Email.Equals(email) && u.Password.Equals(password))
                    .Include(u => u.Address).FirstOrDefaultAsync();
                
                if (user != null)
                {
                    string accountType = user.Address != null ? "PageOwner" : "RegularUser";
                    Console.WriteLine("User " + user.Id + " added to online user ids");
                    if(!onlineUserIds.Contains(user.Id))
                        onlineUserIds.Add(user.Id);
                    return new UserShortVersion
                    {
                        UserId = user.Id,
                        UserFullName = user.Name,
                        AccountType = accountType,
                    };
                }

                Administrator admin = await ctx.Administrators.FirstOrDefaultAsync(a =>
                    a.Email.Equals(email) && a.Password.Equals(password));
                if (admin != null)
                    return new UserShortVersion
                    {
                        UserId = admin.Id,
                        AccountType = "Administrator",
                        UserFullName = admin.Email
                    };

                return null;
            }
        }

        public async Task<UserSocketsModel> GetUserByIdAsync(int senderId, int receiverId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                bool[] bools = new bool[8];
                bool areFriends = UsersAreFriends(senderId, receiverId);
                bools[0] = areFriends;
                UserAction userAction = await ctx.UserActions.FirstOrDefaultAsync
                    (ua => ua.SenderId == senderId && ua.ReceiverId == receiverId);
                if (userAction != null) {
                    bools[1] = userAction.IsFriendRequest;
                    bools[2] = userAction.IsReport;
                    bools[5] = userAction.IsFollowPage;
                }
                else {
                    bools[1] = false;
                    bools[2] = false;
                    bools[5] = false;
                }

                userAction = await ctx.UserActions.FirstOrDefaultAsync(ua =>
                    ua.SenderId == receiverId && ua.ReceiverId == senderId);
                if (userAction == null) {
                    bools[3] = false;
                    bools[4] = false;
                    bools[6] = false;
                }
                else {
                    bools[3] = userAction.IsShareTrainings;
                    bools[4] = userAction.IsShareDiets;
                    bools[6] = userAction.IsFriendRequest;
                }

                bools[7] = false;
                if (IsUserPage(receiverId))
                {
                    PageRating pageRating = await ctx.PageRatings.FirstOrDefaultAsync(pr => pr.UserId == senderId
                        && pr.PageId == receiverId);
                    bools[7] = pageRating != null;
                }
                
                if (IsUserPage(senderId))
                    bools[5] = userAction?.IsFollowPage ?? false;
                
                User user = await ctx.Users.Where(u => u.Id == receiverId)
                    .Include(u => u.Address).FirstAsync();

                int relevantFriendsNumber;
                if (!IsUserPage(senderId) && !IsUserPage(receiverId))
                    relevantFriendsNumber = areFriends
                        ? GetTotalNumberOfFriendsForUser(receiverId)
                        : GetNumberOfCommonFriends(senderId, receiverId);
                else if (IsUserPage(senderId) && !IsUserPage(receiverId))
                    relevantFriendsNumber = UserFollowsGym(receiverId, senderId)
                        ? GetTotalNumberOfFriendsForUser(receiverId) : 0;
                else 
                    relevantFriendsNumber = GetTotalNumberOfFollowersForGym(receiverId);
                
                UserSocketsModel userSocketsModel = new UserSocketsModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Description = user.Description,
                    City = user.City,
                    Score = user.Score,
                    Address = user.Address,
                    UserStatus = bools,
                    RelevantFriendsNumber = relevantFriendsNumber,
                    Rating = 0
                };

                if (!IsUserPage(receiverId))
                    return userSocketsModel;

                double averageRating;
                if (ctx.PageRatings.Any(pr => pr.PageId == receiverId))
                    averageRating = ctx.PageRatings.Where(pr => pr.PageId == receiverId).Average(pr => pr.Rating);
                else
                    averageRating = 0;
                Console.WriteLine("Page average rating is " + averageRating);
                userSocketsModel.Rating = Convert.ToInt32(averageRating);
                return userSocketsModel;
            }
        }

        private bool UsersAreFriends(int firstUserId,int secondUserId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return ctx.Friendships.Any
                (fr => fr.FirstUserId == firstUserId && fr.SecondUserId == secondUserId
                       || fr.FirstUserId == secondUserId && fr.SecondUserId == firstUserId);
            }
        }
        
        private int GetTotalNumberOfFriendsForUser(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return ctx.Friendships.Count(fr => fr.FirstUserId == userId || fr.SecondUserId == userId);
            }
        }
        
        private int GetTotalNumberOfFollowersForGym(int gymId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return ctx.UserActions.Count(ua => ua.ReceiverId == gymId && ua.IsFollowPage);
            }
        }
        
        private int GetNumberOfCommonFriends(int firstUserId,int secondUserId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<int> firstUserFriendIds = new List<int>();
                List<int> temp = ctx.Friendships.Where(fr => fr.FirstUserId == firstUserId)
                    .Select(fr => fr.SecondUserId).ToList();
                firstUserFriendIds.AddRange(temp);
                temp = ctx.Friendships.Where(fr => fr.SecondUserId == firstUserId)
                    .Select(fr => fr.FirstUserId).ToList();
                firstUserFriendIds.AddRange(temp);
                
                
                List<int> secondUserFriendIds = new List<int>();
                temp = ctx.Friendships.Where(fr => fr.FirstUserId == secondUserId)
                    .Select(fr => fr.SecondUserId).ToList();
                secondUserFriendIds.AddRange(temp);
                temp = ctx.Friendships.Where(fr => fr.SecondUserId == secondUserId)
                    .Select(fr => fr.FirstUserId).ToList();
                secondUserFriendIds.AddRange(temp);

                int numberOfCommonFriends = 0;
                foreach (var friendId in firstUserFriendIds)
                {
                    if (secondUserFriendIds.Contains(friendId))
                        numberOfCommonFriends++;
                }

                return numberOfCommonFriends;
            }
        }

        private bool IsUserPage(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                User user = ctx.Users.Where(u => u.Id == userId)
                    .Include(u => u.Address).First();
                Console.WriteLine("User " + userId + " is page " + (user.Address != null));
                return user.Address != null;
            }
        }

        private bool UserFollowsGym(int userId, int gymId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return ctx.UserActions.Any(ua => ua.SenderId == userId && ua.ReceiverId == gymId && ua.IsFollowPage);
            } 
        }

        public async Task<bool> EditUserAsync(UserSocketsModel user)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                User userDb = await ctx.Users.FirstAsync(u => u.Id == user.Id);
                if (user.Email != null)
                    userDb.Email = user.Email;
                else //this means that the request is only supposed to edit the bg picture, without making changes to the db
                    return true;

                if (user.City != null)
                    userDb.City = user.City;
                if (user.Description != null)
                    userDb.Description = user.Description;
                if (user.Name != null)
                    userDb.Name = user.Name;
                if (user.Score != 0)
                    userDb.Score = user.Score;
                if (user.Address != null)
                    userDb.Address = user.Address;
                if (user.Password != null)
                    userDb.Password = user.Password;

                try {
                    ctx.Users.Update(userDb);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return false;
                }
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try {
                    User user = await ctx.Users.FirstAsync(u => u.Id == userId);
                    ctx.Users.Remove(user);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return false;
                }

                return true;
            }
        }

        public async Task<int> PostUserActionAsync(ModelActionSockets modelActionSockets)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                UserAction userAction = await ctx.UserActions.FirstOrDefaultAsync(ua =>
                    ua.SenderId == modelActionSockets.SenderId
                    && ua.ReceiverId == modelActionSockets.ReceiverId);

                UserAction ifNullUserAction = new UserAction();
                if (userAction == null)
                    ifNullUserAction = new UserAction
                    {
                        SenderId = modelActionSockets.SenderId,
                        ReceiverId = modelActionSockets.ReceiverId
                    };

                if (modelActionSockets.Value.ToString().Equals("False"))
                    modelActionSockets.Value = false;
                if (modelActionSockets.Value.ToString().Equals("True"))
                    modelActionSockets.Value = true;

                int returnId = 0;
                switch (modelActionSockets.ActionType)
                {
                    case "USER_FRIEND_REQUEST_SEND":
                        if (userAction != null)
                            userAction.IsFriendRequest = (bool) modelActionSockets.Value;
                        else
                            ifNullUserAction.IsFriendRequest = (bool) modelActionSockets.Value;
                        if ((bool) modelActionSockets.Value)
                        {
                            returnId = await AddNotification("USER_FRIEND_REQUEST_SEND", modelActionSockets.SenderId, modelActionSockets.ReceiverId);
                            Console.WriteLine("Returning notification id: " + returnId);
                        }
                        else {
                            Notification notification = await ctx.Notifications.FirstAsync(n =>
                                n.SenderId == modelActionSockets.SenderId && n.ReceiverId == modelActionSockets.ReceiverId
                                && n.NotificationType.Equals("USER_FRIEND_REQUEST_SEND"));
                            ctx.Notifications.Remove(notification);
                        }
                        break;
                    case "USER_FRIEND_REQUEST_RESPONSE":
                        if (userAction != null)
                        {
                            userAction.IsFriendRequest = false;
                            if ((bool) modelActionSockets.Value)
                                await ctx.Friendships.AddAsync(new Friendship
                                {
                                    FirstUserId = modelActionSockets.SenderId,
                                    SecondUserId = modelActionSockets.ReceiverId
                                });
                        }

                        break;
                    case "USER_SHARE_TRAININGS":
                        if (userAction != null)
                            userAction.IsShareTrainings = (bool) modelActionSockets.Value;
                        else
                            ifNullUserAction.IsShareTrainings = (bool) modelActionSockets.Value;
                        break;
                    case "USER_SHARE_DIETS":
                        if (userAction != null)
                            userAction.IsShareDiets = (bool) modelActionSockets.Value;
                        else
                            ifNullUserAction.IsShareDiets = (bool) modelActionSockets.Value;
                        break;
                    case "USER_FOLLOW_PAGE":
                        if (userAction != null)
                            userAction.IsFollowPage = (bool) modelActionSockets.Value;
                        else
                            ifNullUserAction.IsFollowPage = (bool) modelActionSockets.Value;
                        if ((bool) modelActionSockets.Value)
                        {
                            returnId = await AddNotification("USER_FOLLOW_PAGE", modelActionSockets.SenderId, modelActionSockets.ReceiverId);
                            Console.WriteLine("Returning notification id: " + returnId);
                        }
                        else {
                            Notification notification = await ctx.Notifications.FirstAsync(n =>
                                n.SenderId == modelActionSockets.SenderId && n.ReceiverId == modelActionSockets.ReceiverId
                                                                          && n.NotificationType.Equals("USER_FOLLOW_PAGE"));
                            ctx.Notifications.Remove(notification);
                        }
                        break;
                    case "USER_REPORT":
                        if (userAction != null)
                            userAction.IsReport = (bool) modelActionSockets.Value;
                        else
                            ifNullUserAction.IsReport = (bool) modelActionSockets.Value;
                        break;
                }

                if (userAction == null)
                    await ctx.UserActions.AddAsync(ifNullUserAction);
                else
                {
                    if (userAction.IsReport || userAction.IsFollowPage || userAction.IsFriendRequest
                        || userAction.IsShareDiets || userAction.IsShareTrainings)
                        ctx.UserActions.Update(userAction);
                    else
                        ctx.UserActions.Remove(userAction);
                }

                try
                {
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return -1;
                }

                return returnId;
            }
        }

        private async Task<int> AddNotification(string notificationType, int senderId, int receiverId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                await ctx.Notifications.AddAsync(new Notification {
                    SenderId = senderId,
                    ReceiverId = receiverId,
                    NotificationType = notificationType
                });
                await ctx.SaveChangesAsync();
                return ctx.Notifications.First(n => n.SenderId == senderId && n.ReceiverId == receiverId
                    && n.NotificationType.Equals(notificationType)).Id;
            }
        }

        public async Task<int> RemoveFriendshipAsync(ModelActionSockets modelActionSockets)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    Friendship friendship = await ctx.Friendships.FirstAsync(fr =>
                        (fr.FirstUserId == modelActionSockets.SenderId &&
                         fr.SecondUserId == modelActionSockets.ReceiverId)
                        || (fr.FirstUserId == modelActionSockets.ReceiverId &&
                            fr.SecondUserId == modelActionSockets.SenderId));
                    ctx.Friendships.Remove(friendship);
                    await ctx.SaveChangesAsync();
                    return 0;
                }
                catch (Exception e)
                {
                    return -1;
                }
            }
        }

        public async Task<int> PostPageRatingAsync(ModelActionSockets modelActionSockets)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                PageRating pageRating = await ctx.PageRatings.FirstOrDefaultAsync(pr =>
                    pr.UserId == modelActionSockets.SenderId && pr.PageId == modelActionSockets.ReceiverId);
                if (pageRating == null)
                {
                    await ctx.PageRatings.AddAsync(new PageRating
                    {
                        UserId = modelActionSockets.SenderId,
                        PageId = modelActionSockets.ReceiverId,
                        Rating = (int) modelActionSockets.Value
                    });
                }
                else
                {
                    pageRating.Rating = (int) modelActionSockets.Value;
                    ctx.PageRatings.Update(pageRating);
                }

                await ctx.SaveChangesAsync();
                return 0;
            }
        }

        public async Task<bool> DeleteNotificationAsync(int notificationId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    Notification notification = await ctx.Notifications.FirstAsync(n => n.Id == notificationId);
                    ctx.Notifications.Remove(notification);
                    await ctx.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public List<int> GetPostIdsForUser(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return ctx.Posts.Where(p => p.Owner.Id == userId)
                    .Select(p => p.Id).ToList();
            }
        }

        public List<SearchBarUser> GetUsersByFilter(string filterString)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<User> usersDb = ctx.Users.Where(u => u.Name.ToLower().StartsWith(filterString.ToLower())).ToList();
                List<SearchBarUser> users = new List<SearchBarUser>();
                foreach (var user in usersDb)
                {
                    users.Add(new SearchBarUser {
                        UserId = user.Id,
                        FullName = user.Name
                    });
                }

                return users;
            }
        }

        public List<UserShortVersion> GetAllGymsInCity(string city)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<User> gymsInCityDb = ctx.Users.Where(u => u.Address != null && u.City.Equals(city)).ToList();
                List<UserShortVersion> gyms = new List<UserShortVersion>();
                foreach (var gymDb in gymsInCityDb)
                {
                    gyms.Add(new UserShortVersion
                    {
                        UserId = gymDb.Id,
                        UserFullName = gymDb.Name
                    });
                }

                return gyms;
            }
        }

        public List<NotificationSockets> GetNotificationsForUser(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<Notification> notificationsDb = ctx.Notifications.Where(n => n.ReceiverId == userId).ToList();
                List<NotificationSockets> notifications = new List<NotificationSockets>();
                foreach (var notification in notificationsDb)
                {
                    string userName = ctx.Users.First(u => u.Id == notification.SenderId).Name;
                    notifications.Add(new NotificationSockets
                    {
                        Id = notification.Id,
                        ActionType = notification.NotificationType,
                        ReceiverId = notification.ReceiverId,
                        SenderId = notification.SenderId,
                        SenderName = userName
                    });
                }

                return notifications;
            }
        }

        public List<UserShortVersionWithStatus> GetFriendsForUser(int userId, int senderId, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<User> friendsDb;
                if (!IsUserPage(userId) && !IsUserPage(senderId)) // user goes to user profile
                {
                    if (UsersAreFriends(userId, senderId) || userId == senderId)
                        friendsDb = GetFriendListForUser(userId);
                    else
                        friendsDb = GetCommonFriendsForUsers(userId, senderId);
                }
                else if (IsUserPage(userId) && !IsUserPage(senderId)) // user goes to gym profile
                {
                    if (UserFollowsGym(senderId, userId))
                        friendsDb = GetCommonFollowersFriends(senderId, userId);
                    else
                        friendsDb = null;
                }
                else if (!IsUserPage(userId) && IsUserPage(senderId)) // gym goes to user profile
                {
                    if (UserFollowsGym(userId, senderId))
                        friendsDb = GetFriendListForUser(userId);
                    else
                        friendsDb = null;
                }
                else
                    friendsDb = GetCommonFollowersForGyms(userId, senderId);


                if (friendsDb == null)
                    return null;
                
                if (offset >= friendsDb.Count)
                    return null;

                List<User> friendsDbSorted = friendsDb.OrderBy(u => u.Name).ToList();
                List<UserShortVersionWithStatus> friends = new List<UserShortVersionWithStatus>();
                for (int i = offset; i < offset + 10; i++)
                {
                    if(i >= friendsDbSorted.Count)
                        break;
                    
                    bool[] status = new bool[2];
                    if(userId == senderId)
                        status = GetStatusForUser(senderId, friendsDbSorted[i].Id);
                    friends.Add(new UserShortVersionWithStatus
                    {
                        UserId = friendsDbSorted[i].Id,
                        UserFullName = friendsDbSorted[i].Name,
                        Status = status
                    });
                }

                return friends;
            }
        }

        public async Task<bool> IncrementUserScoreAsync(int userId, int amount)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    User user = await ctx.Users.FirstAsync(u => u.Id == userId);
                    user.Score += amount;
                    ctx.Users.Update(user);
                    await ctx.SaveChangesAsync();
                    return true;
                }
                catch (Exception e) {
                    return false;
                }
            }
        }

        public List<UserShortVersion> GetOnlineFriendsForUser(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                // List<User> userFriends = new List<User>();
                // List<User> temp = ctx.Friendships.Where(fr => fr.FirstUserId == userId)
                //     .Select(fr => fr.SecondUser).ToList();
                // userFriends.AddRange(temp);
                // temp = ctx.Friendships.Where(fr => fr.SecondUserId == userId)
                //     .Select(fr => fr.FirstUser).ToList();
                // userFriends.AddRange(temp);
                //
                // for (int i = 0; i < userFriends.Count; i++)
                // {
                //     if (!onlineUserIds.Contains(userFriends[i].Id))
                //     {
                //         userFriends.RemoveAt(i);
                //         i--;
                //     }
                // }
                List<int> onlineFriendIds = new List<int>();
                foreach (var onlineUserId in onlineUserIds) {
                    if(UsersAreFriends(userId, onlineUserId))
                        onlineFriendIds.Add(onlineUserId);
                }

                if (!onlineFriendIds.Any())
                    return null;
                
                List<UserShortVersion> onlineFriends = new List<UserShortVersion>();
                foreach (var onlineFriendId in onlineFriendIds) {
                    string username = ctx.Users.First(u => u.Id == onlineFriendId).Name;
                    onlineFriends.Add(new UserShortVersion
                    {
                        UserId = onlineFriendId,
                        UserFullName = username
                    });
                }
                return onlineFriends;
            }
        }

        public List<int> LogoutOrInUser(int userId, bool isLogout)
        {
            Console.WriteLine("Logging out or in user with id " + userId + " is logout " + isLogout);
            if(isLogout)
                onlineUserIds.Remove(userId);
            
            List<int> onlineUserFriendIds = new List<int>();
            foreach (var onlineUserId in onlineUserIds)
            {
                if (UsersAreFriends(userId, onlineUserId))
                    onlineUserFriendIds.Add(onlineUserId);
            }

            return onlineUserFriendIds;
        }

        public UserShortVersion GetUserShortVersionById(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try {
                    string username = ctx.Users.First(u => u.Id == userId).Name;
                    return new UserShortVersion
                    {
                        UserId = userId,
                        UserFullName = username
                    };
                }
                catch (Exception e) {
                    return null;
                }
            }
        }

        private bool[] GetStatusForUser(int senderId, int targetUserId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                bool[] status = new bool[2];
                try
                {
                    UserAction userAction = ctx.UserActions.First(ua => ua.SenderId == senderId && ua.ReceiverId == targetUserId);
                    status[0] = userAction.IsShareTrainings;
                    status[1] = userAction.IsShareDiets;
                }
                catch (Exception e) {}
                return status;
            }
        }

        private List<User> GetCommonFriendsForUsers(int firstUserId, int secondUserId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<User> firstUserFriends = new List<User>();
                List<User> temp = ctx.Friendships.Where(fr => fr.FirstUserId == firstUserId)
                    .Select(fr => fr.SecondUser).ToList();
                firstUserFriends.AddRange(temp);
                temp = ctx.Friendships.Where(fr => fr.SecondUserId == firstUserId)
                    .Select(fr => fr.FirstUser).ToList();
                firstUserFriends.AddRange(temp);
                
                List<User> secondUserFriends = new List<User>();
                temp = ctx.Friendships.Where(fr => fr.FirstUserId == secondUserId)
                    .Select(fr => fr.SecondUser).ToList();
                secondUserFriends.AddRange(temp);
                temp = ctx.Friendships.Where(fr => fr.SecondUserId == secondUserId)
                    .Select(fr => fr.FirstUser).ToList();
                secondUserFriends.AddRange(temp);

                List<User> commonFriends = new List<User>();
                foreach (var friend in firstUserFriends)
                {
                    if (secondUserFriends.Contains(friend))
                        commonFriends.Add(friend);
                }

                return commonFriends;
            }
        }

        private List<User> GetFriendListForUser(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<User> friendsDb = ctx.Friendships.Where(fr => fr.FirstUserId == userId)
                    .Select(fr => fr.SecondUser).ToList();
                List<User> temp = ctx.Friendships.Where(fr => fr.SecondUserId == userId)
                    .Select(fr => fr.FirstUser).ToList();
                friendsDb.AddRange(temp);
                return friendsDb;
            }
        }
        
        private List<User> GetCommonFollowersFriends(int userId, int gymId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<User> userFriends = new List<User>();
                userFriends.Add(ctx.Users.First(u => u.Id == userId));
                List<User> temp = ctx.Friendships.Where(fr => fr.FirstUserId == userId)
                    .Select(fr => fr.SecondUser).ToList();
                userFriends.AddRange(temp);
                temp = ctx.Friendships.Where(fr => fr.SecondUserId == userId)
                    .Select(fr => fr.FirstUser).ToList();
                userFriends.AddRange(temp);
                
                List<User> gymFollowers = ctx.UserActions.Where(ua => ua.ReceiverId == gymId && ua.IsFollowPage)
                    .Select(ua => ua.Sender).ToList();
                
                List<User> commonFollowersFriends = new List<User>();
                foreach (var gymFollower in gymFollowers)
                {
                    if(userFriends.Contains(gymFollower))
                        commonFollowersFriends.Add(gymFollower);
                }

                return commonFollowersFriends;
            } 
        }

        private List<User> GetCommonFollowersForGyms(int firstGymId, int secondGymId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<User> firstGymFollowers = ctx.UserActions.Where(ua => ua.ReceiverId == firstGymId && ua.IsFollowPage)
                    .Select(ua => ua.Sender).ToList();
                
                List<User> secondGymFollowers = ctx.UserActions.Where(ua => ua.ReceiverId == secondGymId && ua.IsFollowPage)
                    .Select(ua => ua.Sender).ToList();
                
                List<User> commonFollowers = new List<User>();
                foreach (var gymFollower in firstGymFollowers)
                {
                    if(secondGymFollowers.Contains(gymFollower))
                        commonFollowers.Add(gymFollower);
                }

                return commonFollowers;
            } 
        }
    }
}