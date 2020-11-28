using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SEP3_T3.Persistance;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories.Implementation
{
    public class PostRepo : IPostRepo
    {
        public async Task<int> AddPostAsync(PostShortVersion postShortVersion)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                User owner = await ctx.Users.FirstAsync(u => u.Id == postShortVersion.Owner.UserId);
                Post post = new Post
                {
                    Title = postShortVersion.Title,
                    Content = postShortVersion.Content,
                    Owner = owner,
                    TimeStamp = DateTime.Now,
                    HasImage = postShortVersion.HasImage
                };
                await ctx.Posts.AddAsync(post);
                await ctx.SaveChangesAsync();
                return ctx.Posts.LastAsync().Id;
            }
        }

        public async Task<Post> GetPostByIdAsync(int postId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return await ctx.Posts.Where(p => p.Id == postId)
                    .Include(p => p.Comments).FirstAsync();
            }
        }

        public async Task<bool> EditPostAsync(PostShortVersion post)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Post postDb = await ctx.Posts.FirstAsync(p => p.Id == post.Id);
                if (post.Title != null)
                    postDb.Title = post.Title;
                if (post.Content != null)
                    postDb.Content = post.Content;
                try {
                    ctx.Posts.Update(postDb);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return false;
                }

                return true;
            }
        }
        
        public async Task<bool> DeletePostAsync(int postId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try {
                    Post post = await ctx.Posts.FirstAsync(p => p.Id == postId);
                    ctx.Posts.Remove(post);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return false;
                }

                return true;
            }
        }

        public async Task<List<PostShortVersion>> GetLatestPostsForUserAsync(int userId, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<Post> postsDb = ctx.Posts.Where(p => 
                    p.Owner.Id == userId || IsFriends(userId, p.Owner.Id))
                    .OrderByDescending(p => p.TimeStamp).ToList();

                if (postsDb.Count < offset)
                    return null;

                return GetPosts(offset, postsDb);
            }
        }

        public async Task<List<PostShortVersion>> GetLatestPostsByUser(int userId, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<Post> postsDb = ctx.Posts.Where(p => p.Owner.Id == userId)
                    .OrderByDescending(p => p.TimeStamp).ToList();
                
                if (postsDb.Count < offset)
                    return null;

                return GetPosts(offset, postsDb);
            }
        }

        public async Task<int> PostPostActionAsync(PostActionSockets postActionSockets)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                PostAction postAction = await ctx.PostActions.FirstOrDefaultAsync(pa =>
                    pa.PostId == postActionSockets.PostId && pa.UserId == postActionSockets.UserId);

                PostAction ifNullPostAction = new PostAction();
                if (postAction == null)
                {
                    ifNullPostAction = new PostAction
                    {
                        PostId = postActionSockets.PostId,
                        UserId = postActionSockets.UserId
                    };
                }

                switch (postActionSockets.ActionType)
                {
                    case "POST_LIKE":
                        if (postAction != null)
                            postAction.IsLike = postActionSockets.Value;
                        else
                            ifNullPostAction.IsLike = postActionSockets.Value;
                        break;
                    case "POST_REPORT":
                        if (postAction != null)
                            postAction.IsReport = postActionSockets.Value;
                        else
                            ifNullPostAction.IsReport = postActionSockets.Value;
                        break;
                }

                if (postAction == null)
                    await ctx.PostActions.AddAsync(ifNullPostAction);
                else
                {
                    if (postAction.IsLike || postAction.IsReport)
                        ctx.PostActions.Update(postAction);
                    else
                        ctx.PostActions.Remove(postAction);
                }
                
                try {
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return -1;
                }

                return 0;
            }
        }

        public async Task<int> AddCommentToPost(CommentForPost comment)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Post post = await ctx.Posts.FirstAsync(p => p.Id == comment.PostId);
                User owner = await ctx.Users.FirstAsync(u => u.Id == comment.OwnerId);
                Comment commentDb = new Comment
                {
                    Content = comment.Content,
                    TimeStamp = DateTime.Now.ToString(),
                    Owner = owner
                };
                post.Comments.Add(commentDb);
                ctx.Posts.Update(post);
                await ctx.Comments.AddAsync(commentDb);
                await ctx.SaveChangesAsync();
                return ctx.Comments.Last().Id;
            }
        }

        public async Task<bool> DeleteCommentFromPost(int commentId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Comment comment = await ctx.Comments.FirstAsync(c => c.Id == commentId);
                try {
                    ctx.Comments.Remove(comment);
                    await ctx.SaveChangesAsync();
                    return true;
                }
                catch (Exception e) {
                    return false;
                }
            }
        }

        public async Task<List<CommentSockets>> GetAllCommentsForPost(int postId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Post post = await ctx.Posts.Where(p => p.Id == postId)
                    .Include(p => p.Comments).FirstAsync();
                if (post.Comments != null && post.Comments.Any())
                {
                    List<CommentSockets> comments = new List<CommentSockets>();
                    foreach (var postComment in post.Comments)
                    {
                        UserShortVersion owner = new UserShortVersion {
                            UserId = postComment.Owner.Id,
                            UserFullName = postComment.Owner.Name
                        };
                        comments.Add(new CommentSockets
                        {
                         Id = postComment.Id,
                         Owner = owner,
                         Content = postComment.Content,
                         TimeStamp = postComment.TimeStamp
                        });
                    }

                    return comments;
                }

                return null;
            }
        }

        public async Task<List<UserShortVersion>> GetAllLikesForPost(int postId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try {
                    List<User> postLikeUsers = ctx.PostActions.Where(pa =>
                        pa.PostId == postId && pa.IsLike).Select(pa => pa.User).ToList();
                    
                    if (!postLikeUsers.Any())
                        return null;
                
                    List<UserShortVersion> users = new List<UserShortVersion>();
                    foreach (var user in postLikeUsers)
                    {
                        users.Add(new UserShortVersion {
                            UserId = user.Id,
                            UserFullName = user.Name
                        });
                    }
                    return users;
                }
                catch (Exception e) {
                    return null;
                }
            }
        }

        private bool IsFriends(int user1Id, int user2Id)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Friendship friendship = ctx.Friendships.FirstOrDefault(fr =>
                    (fr.FirstUserId == user1Id && fr.SecondUserId == user2Id)
                    || (fr.FirstUserId == user2Id && fr.SecondUserId == user1Id));
                
                return friendship != null;
            }
        }

        private List<PostShortVersion> GetPosts(int offset, List<Post> postsDb)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<PostShortVersion> posts = new List<PostShortVersion>();
                for (int i = offset; i < offset + 5; i++)
                {
                    if(i >= postsDb.Count)
                        break;

                    UserShortVersion owner = new UserShortVersion {
                        UserId = postsDb[i].Owner.Id,
                        UserFullName = postsDb[i].Owner.Name
                    };
                    posts.Add(new PostShortVersion {
                        Id = postsDb[i].Id,
                        Title = postsDb[i].Title,
                        Content = postsDb[i].Content,
                        Owner = owner,
                        TimeStamp = postsDb[i].TimeStamp,
                        HasImage = postsDb[i].HasImage
                    });
                }

                foreach (var post in posts)
                {
                    int countComments = ctx.Posts.Where(p => p.Id == post.Id)
                        .Select(p => p.Comments).Count();
                    Console.WriteLine("Post " + post.Id + " has " + countComments + " comments");
                    post.NumberOfComments = countComments;

                    int countLikes = ctx.PostActions.Count(pa => pa.PostId == post.Id && pa.IsLike);
                    Console.WriteLine("Post " + post.Id + " has " + countLikes + " likes");
                    post.NumberOfLikes = countLikes;
                }

                return posts;
            }
        }
    }
}