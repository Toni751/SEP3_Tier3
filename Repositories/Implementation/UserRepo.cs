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
                User user = await ctx.Users.FirstOrDefaultAsync(u =>
                    u.Email.Equals(email) && u.Password.Equals(password));
                if (user != null)
                {
                    string accountType = user.Address != null ? "PageOwner" : "RegularUser";

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

        public async Task<List<Post>> GetLatestPostsForUserAsync(int id, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
               //return await ctx.Posts.Where(p => p.Owner.Id == id).Take(5).ToListAsync();
            }

            return null;
        }

        public async Task<UserSocketsModel> GetUserByIdAsync(int senderId, int receiverId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                bool[] bools = new bool[7];
                bool areFriends = ctx.Friendships.Any
                (fr => fr.FirstUserId == senderId && fr.SecondUserId == receiverId
                       || fr.FirstUserId == receiverId && fr.SecondUserId == senderId);
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

                User user = await ctx.Users.Where(u => u.Id == receiverId)
                    .Include(u => u.Address).FirstAsync();
                return new UserSocketsModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Description = user.Description,
                    City = user.City,
                    Score = user.Score,
                    Address = user.Address,
                    UserStatus = bools
                };
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
    }
}