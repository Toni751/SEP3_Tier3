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
    /// <summary>
    /// The post repository class for accessing the database for posts requests
    /// </summary>
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
                Console.WriteLine("adding post to db");
                await ctx.Posts.AddAsync(post);
                await ctx.SaveChangesAsync();
                Console.WriteLine("post saved");
                var newPost = await ctx.Posts.ToListAsync();
                return newPost.Last().Id;
            }
        }

        public async Task<PostShortVersion> GetPostByIdAsync(int postId, int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Post post = await ctx.Posts
                    .Include(p => p.Owner)
                    .FirstOrDefaultAsync(p => p.Id == postId);
                
                if (post == null)
                    return null;
                
                UserShortVersion owner = new UserShortVersion
                {
                    UserId = post.Owner.Id,
                    UserFullName = post.Owner.Name,
                };
                int countComments = ctx.Posts.Where(p => p.Id == post.Id)
                    .Include(p => p.Comments).First().Comments.Count;
                Console.WriteLine("Post " + post.Id + " has " + countComments + " comments");

                int countLikes = ctx.PostActions.Count(pa => pa.PostId == post.Id && pa.IsLike);
                Console.WriteLine("Post " + post.Id + " has " + countLikes + " likes");

                PostAction postAction = await ctx.PostActions.FirstOrDefaultAsync(pa =>
                    pa.PostId == postId && pa.UserId == userId);
                bool[] postStatus = new bool[2];

                if (postAction == null) {
                    for (int i = 0; i < postStatus.Length; i++) {
                        postStatus[i] = false;
                    }
                }
                else {
                    postStatus[0] = postAction.IsLike;
                    postStatus[1] = postAction.IsReport;
                }
                
                return new PostShortVersion
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Owner = owner,
                    TimeStamp = post.TimeStamp,
                    HasImage = post.HasImage,
                    NumberOfComments = countComments,
                    NumberOfLikes = countLikes,
                    PostStatus = postStatus
                };
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
                    Post post = await ctx.Posts.Where(p => p.Id == postId)
                        .Include(p => p.Comments).FirstAsync();
                    ctx.Posts.Remove(post);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return false;
                }

                return true;
            }
        }

        public List<int> GetLatestPostsForUserAsync(int userId, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<int> userIds = GetFriendsIdsForUserWithUserId(userId);
                userIds.AddRange(GetFollowingGymsIdsForUserId(userId));
                List<Post> postsDb = new List<Post>();

                foreach (var id in userIds)
                {
                    try {
                        var posts = ctx.Posts.Where(p => p.Owner.Id == id).ToList();
                       postsDb.AddRange(posts);
                    }
                    catch (Exception e) {
                        Console.WriteLine("No posts found by user " + id);
                    }
                }

                var postsDbSorted = postsDb.OrderByDescending(p => p.TimeStamp)
                    .Select(p => p.Id).ToList();
                
                if (postsDbSorted.Count <= offset)
                    return null;
                
                List<int> postIds = new List<int>();
                for (int i = offset; i < offset + 5; i++)
                {
                    if(i >= postsDb.Count)
                        break;
                    
                    postIds.Add(postsDbSorted[i]);
                }
                return postIds;
                
            }
        }

        public List<int> GetLatestPostsByUser(int userId, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<int> postsDb = ctx.Posts.Where(p => p.Owner.Id == userId)
                    .OrderByDescending(p => p.TimeStamp)
                    .Select(p => p.Id).ToList();
                
                if (postsDb.Count <= offset)
                    return null;

                List<int> postIds = new List<int>();
                for (int i = offset; i < offset + 5; i++)
                {
                    if(i >= postsDb.Count)
                        break;
                    
                    postIds.Add(postsDb[i]);
                }
                return postIds;
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
                Post post = await ctx.Posts.Where(p => p.Id == comment.PostId)
                    .Include(p => p.Comments).FirstAsync();
                User owner = await ctx.Users.FirstAsync(u => u.Id == comment.OwnerId);
                Comment commentDb = new Comment
                {
                    Content = comment.Content,
                    TimeStamp = DateTime.Now,
                    Owner = owner
                };
                post.Comments.Add(commentDb);
                ctx.Posts.Update(post);
                await ctx.Comment.AddAsync(commentDb);
                await ctx.SaveChangesAsync();
                return ctx.Comment.Max(c => c.Id);
            }
        }

        public async Task<bool> DeleteCommentFromPost(int commentId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Comment comment = await ctx.Comment.FirstAsync(c => c.Id == commentId);
                try {
                    ctx.Comment.Remove(comment);
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
                    .Include(p => p.Comments)
                    .ThenInclude(c => c.Owner)
                    .Include(p => p.Owner).FirstAsync();

                List<Comment> orderedComments = post.Comments.OrderByDescending(c => c.TimeStamp).ToList();
                if (orderedComments.Any())
                {
                    List<CommentSockets> comments = new List<CommentSockets>();
                    foreach (var postComment in orderedComments)
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

        public List<UserShortVersion> GetAllLikesForPost(int postId)
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

        private List<int> GetFriendsIdsForUserWithUserId(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<int> userIds = new List<int>();
                userIds.Add(userId);
                List<int> friendsFirst = ctx.Friendships.Where(fr => fr.FirstUserId == userId)
                    .Select(fr => fr.SecondUserId).ToList();
                userIds.AddRange(friendsFirst);
                List<int> friendsSecond = ctx.Friendships.Where(fr => fr.SecondUserId == userId)
                    .Select(fr => fr.FirstUserId).ToList();
                userIds.AddRange(friendsSecond);
                return userIds;
            }
        }

        private List<int> GetFollowingGymsIdsForUserId(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return ctx.UserActions.Where(ua => ua.SenderId == userId && ua.IsFollowPage)
                    .Select(ua => ua.ReceiverId).ToList();
            }
        }
    }
}