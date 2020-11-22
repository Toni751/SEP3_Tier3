using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SEP3_T3.Persistance;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories.Implementation
{
    public class UserRepo : IUserRepo
    {
        public async Task<int> AddUserAsync(UserSocketsModel user)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
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

                    // byte[] avatar = File.ReadAllBytes("C:/Users/Przemo/RiderProjects/SEP3_Tier3/Images/Avatars/"+user.Id+".png");

                    //  Console.WriteLine("XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX" + avatar.Length);

                    return new UserShortVersion
                    {
                        UserId = user.Id,
                        UserFullName = user.Name,
                        AccountType = accountType,
                    };
                }
                else
                {
                    Administrator admin = await ctx.Administrators.FirstOrDefaultAsync(a =>
                        a.Email.Equals(email) && a.Password.Equals(password));
                    if (admin != null)
                        return new UserShortVersion
                        {
                            UserId = admin.Id,
                            AccountType = "Administrator",
                            UserFullName = admin.Email
                        };

                }

                return null;
            }
        }

        public async Task<List<Post>> GetLatestPostsForUserAsync(int id, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return await ctx.Posts.Where(p => p.Owner.Id == id).Take(5).ToListAsync();
            }
        }

        public async Task<UserSocketsModel> GetUserByIdAsync(int senderId, int receiverId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                bool[] bools = new bool[6];
                bool areFriends = ctx.Friendships.Any
                (fr => fr.FirstUserId == senderId && fr.SecondUserId == receiverId
                       || fr.FirstUserId == receiverId && fr.SecondUserId == senderId);
                bools[0] = areFriends;
                UserAction userAction = await ctx.UserActions.FirstOrDefaultAsync
                    (ua => ua.SenderId == senderId && ua.ReceiverId == receiverId);
                if (userAction != null)
                {
                    bools[1] = userAction.IsFriendRequest;
                    bools[2] = userAction.IsReport;
                    bools[3] = userAction.IsShareTrainings;
                    bools[4] = userAction.IsShareDiets;
                    bools[5] = userAction.IsFollowPage;
                }
                else
                {
                    for (int i = 1; i < bools.Length; i++)
                    {
                        bools[i] = false;
                    }
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

                User userDb = new User
                {
                    Id = user.Id,
                    Address = user.Address,
                    City = user.City,
                    Description = user.Description,
                    Email = user.Email,
                    Name = user.Name,
                    Score = user.Score
                };
                if (user.Password != null)
                    userDb.Password = user.Password;

                ctx.Users.Update(userDb);
                await ctx.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    User user = await ctx.Users.FirstAsync(u => u.Id == userId);
                    ctx.Users.Remove(user);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public async Task<bool> PostUserActionAsync(UserActionSockets userActionSockets)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                UserAction userAction = await ctx.UserActions.FirstOrDefaultAsync(ua =>
                    ua.SenderId == userActionSockets.SenderId
                    && ua.ReceiverId == userActionSockets.ReceiverId);

                UserAction ifNullUserAction = new UserAction();
                if (userAction == null)
                    ifNullUserAction = new UserAction
                    {
                        SenderId = userActionSockets.SenderId,
                        ReceiverId = userActionSockets.ReceiverId
                    };
                switch (userActionSockets.ActionType)
                {
                    case "USER_FRIEND_REQUEST_SEND":
                        if (userAction != null)
                            userAction.IsFriendRequest = (bool) userActionSockets.Value;
                        else
                            ifNullUserAction.IsFriendRequest = (bool) userActionSockets.Value;
                        break;
                    case "USER_FRIEND_REQUEST_RESPONSE":
                        if (userAction != null) 
                        {
                            userAction.IsFriendRequest = false;
                            if ((bool) userActionSockets.Value)
                                await ctx.Friendships.AddAsync(new Friendship {
                                    FirstUserId = userActionSockets.SenderId,
                                    SecondUserId = userActionSockets.ReceiverId
                                });
                        }
                        break;
                    case "USER_SHARE_TRAININGS":
                        if (userAction != null)
                            userAction.IsShareTrainings = (bool) userActionSockets.Value;
                        else
                            ifNullUserAction.IsShareTrainings = (bool) userActionSockets.Value;
                        break;
                    case "USER_SHARE_DIETS":
                        if (userAction != null)
                            userAction.IsShareDiets = (bool) userActionSockets.Value;
                        else
                            ifNullUserAction.IsShareDiets = (bool) userActionSockets.Value;
                        break;
                    case "USER_FOLLOW_PAGE":
                        if (userAction != null)
                            userAction.IsFollowPage = (bool) userActionSockets.Value;
                        else
                            ifNullUserAction.IsFollowPage = (bool) userActionSockets.Value;
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

                await ctx.SaveChangesAsync();
                return true;
            }
        }

        public async Task<bool> RemoveFriendshipAsync(UserActionSockets userActionSockets)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    Friendship friendship = await ctx.Friendships.FirstAsync(fr =>
                        (fr.FirstUserId == userActionSockets.SenderId &&
                         fr.SecondUserId == userActionSockets.ReceiverId)
                        || (fr.FirstUserId == userActionSockets.ReceiverId &&
                            fr.SecondUserId == userActionSockets.SenderId));
                    ctx.Friendships.Remove(friendship);
                    await ctx.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public async Task<bool> PostPageRatingAsync(UserActionSockets userActionSockets)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                PageRating pageRating = await ctx.PageRatings.FirstOrDefaultAsync(pr =>
                    pr.UserId == userActionSockets.SenderId && pr.PageId == userActionSockets.ReceiverId);
                if (pageRating == null)
                {
                    await ctx.PageRatings.AddAsync(new PageRating
                    {
                        UserId = userActionSockets.SenderId,
                        PageId = userActionSockets.ReceiverId,
                        Rating = (int) userActionSockets.Value
                    });
                }
                else
                {
                    pageRating.Rating = (int) userActionSockets.Value;
                    ctx.PageRatings.Update(pageRating);
                }

                await ctx.SaveChangesAsync();
                return true;
            }
        }
    }
}