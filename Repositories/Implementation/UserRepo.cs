using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SEP3_T3.Persistance;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories.Implementation
{
    public class UserRepo : IUserRepo
    {
        public async Task<bool> AddUserAsync(UserSocketsModel user)
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
                Console.WriteLine("Added user to database");
                return true;
            }
        }

        public async Task<UserShortVersion> LoginAsync(string email, string password)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                User user = await ctx.Users.FirstOrDefaultAsync(u => u.Email.Equals(email) && u.Password.Equals(password));
                if (user != null)
                {
                    string accountType = user.Address != null ? "PageOwner" : "RegularUser";

                    return new UserShortVersion
                    {
                        UserId = user.Id,
                        UserFullName = user.Name,
                        AccountType = accountType
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

        public async Task<User> GetUserByIdAsync(int id)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return await ctx.Users.FirstOrDefaultAsync(u => u.Id == id);
            }
        }
    }
}