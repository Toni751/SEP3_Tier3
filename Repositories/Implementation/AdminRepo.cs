using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SEP3_T3.Persistance;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories.Implementation
{
    public class AdminRepo : IAdminRepo
    {
        public async Task<List<UserShortVersion>> GetAdminUsersAsync(int limit, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                var userReportList = ctx.UserActions.Where(u => u.IsReport)
                    .GroupBy(ua => ua.ReceiverId)
                    .Select(group => new
                    {
                        ReportReceiverId = group.Key,
                        ReportCount = group.Count()
                    })
                    .OrderByDescending(ur => ur.ReportCount).ToList();

                List<int> allUserIds = ctx.Users.Select(u => u.Id).ToList();
                List<int> allReportedUserIds = new List<int>();
                
                foreach (var userReport in userReportList)
                {
                    Console.WriteLine($"User {userReport.ReportReceiverId} was reported {userReport.ReportCount} times");
                    allReportedUserIds.Add(userReport.ReportReceiverId);
                }

                for (int i = 0; i < allUserIds.Count; i++)
                {
                    if (!CheckIfIdIsInList(allUserIds[i], allReportedUserIds))
                        userReportList.Add(new {
                            ReportReceiverId = allUserIds[i],
                            ReportCount = 0
                        });
                }

                if (offset > userReportList.Count)
                    return null;

                List<UserShortVersion> users = new List<UserShortVersion>();
                for (int i = offset; i < offset + limit; i++)
                {
                    if (i >= userReportList.Count)
                        break;

                    users.Add(GetUserShortVersionById(userReportList[i].ReportReceiverId));
                }

                return users;
            }
        }

        private bool CheckIfIdIsInList(int id, List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
                if (id == list[i])
                    return true;
            return false;
        }
        public async Task<List<int>> GetAdminPostsAsync(int limit, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                var postReportList = ctx.PostActions.Where(p => p.IsReport)
                    .GroupBy(pa => pa.PostId)
                    .Select(group => new
                    {
                        ReportPostId = group.Key,
                        ReportCount = group.Count()
                    })
                    .OrderByDescending(pr => pr.ReportCount).ToList();

                List<int> allPostIds = ctx.Posts.Select(p => p.Id).ToList();
                List<int> allReportedPostIds = new List<int>();
                foreach (var postReport in postReportList)
                {
                    Console.WriteLine($"User {postReport.ReportPostId} was reported {postReport.ReportCount} times");
                    allReportedPostIds.Add(postReport.ReportPostId);
                }

                for (int i = 0; i < allPostIds.Count; i++)
                {
                    if (!CheckIfIdIsInList(allPostIds[i], allReportedPostIds))
                        postReportList.Add(new {
                            ReportPostId = allPostIds[i],
                            ReportCount = 0
                        });
                }
                
                if (offset > postReportList.Count)
                    return null;

                //List<PostShortVersion> posts = new List<PostShortVersion>();
                List<int> postIds = new List<int>();
                for (int i = offset; i < offset + limit; i++)
                {
                    if (i >= postReportList.Count)
                        break;

                    postIds.Add(postReportList[i].ReportPostId);
                    // Post post = await ctx.Posts.FirstAsync(p => p.Id == postReportList[i].ReportPostId);
                    // posts.Add(new PostShortVersion
                    // {
                    //     Id = post.Id,
                    //     Title = post.Title,
                    //     Content = post.Content,
                    //    // Owner = GetUserShortVersionById(post.Owner.Id),
                    //     TimeStamp = post.TimeStamp
                    // });
                }
                return postIds;
            }
        }

        public async Task<int> GetTotalNumberAsync(string modelType)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                if (modelType.Equals("users"))
                    return await ctx.Users.CountAsync(u => u.Id >= 0);
                return await ctx.Posts.CountAsync(p => p.Id >= 0);
            }
        }

        private UserShortVersion GetUserShortVersionById(int id)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                User user = ctx.Users.First(u => u.Id == id);
                string accountType = user.Address != null ? "PageOwner" : "RegularUser";
                return new UserShortVersion
                {
                    UserId = user.Id,
                    UserFullName = user.Name,
                    AccountType = accountType
                };
            }
        }
    }
}