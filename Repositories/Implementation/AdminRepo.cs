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

                foreach (var userReport in userReportList)
                {
                    Console.WriteLine(
                        $"User {userReport.ReportReceiverId} was reported {userReport.ReportCount} times");
                }

                if (offset > userReportList.Count)
                    return null;

                List<UserShortVersion> users = new List<UserShortVersion>();
                for (int i = offset; i < offset + limit; i++)
                {
                    if (i > userReportList.Count)
                        break;

                    users.Add(GetUserShortVersionById(userReportList[i].ReportReceiverId));
                }

                return users;
            }
        }

        public async Task<List<PostShortVersion>> GetAdminPostsAsync(int limit, int offset)
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

                foreach (var postReport in postReportList)
                {
                    Console.WriteLine($"User {postReport.ReportPostId} was reported {postReport.ReportCount} times");
                }

                if (offset > postReportList.Count)
                    return null;

                List<PostShortVersion> posts = new List<PostShortVersion>();
                for (int i = offset; i < offset + limit; i++)
                {
                    if (i > postReportList.Count)
                        break;

                    Post post = await ctx.Posts.FirstAsync(p => p.Id == postReportList[i].ReportPostId);
                    posts.Add(new PostShortVersion
                    {
                        Id = post.Id,
                        Title = post.Title,
                        Content = post.Content,
                        Owner = GetUserShortVersionById(post.Owner.Id),
                        TimeStamp = post.TimeStamp
                    });
                }

                return posts;
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