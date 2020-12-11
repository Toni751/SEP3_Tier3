using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SEP3_T3.Persistance;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories.UnitTestInterfaces;

namespace SEP3_Tier3.Repositories.Implementation
{
    /// <summary>
    /// The chat repository class for accessing the database for chat requests
    /// </summary>
    public class ChatRepo : IChatRepo, IChatRepoTest
    {
        public async Task<List<int>> AddMessageAsync(MessageSocketsModel message)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<int> ints = new List<int>();
                if (message.HasImage && message.Content == null)
                    message.Content = "";
                await ctx.Messages.AddAsync(new Message
                {
                    Content = message.Content,
                    HasImage = message.HasImage,
                    ReceiverId = message.ReceiverId,
                    SenderId = message.SenderId,
                    TimeStamp = DateTime.Now
                });
                await ctx.SaveChangesAsync();
                ints.Add(ctx.Messages.ToList().Last().Id);
                
                await ctx.Notifications.AddAsync(new Notification {
                    SenderId = message.SenderId,
                    ReceiverId = message.ReceiverId,
                    NotificationType = ActionType.MESSAGE_CREATE.ToString()
                });
                await ctx.SaveChangesAsync();
                ints.Add(ctx.Notifications.ToList().Last().Id);
                
                return ints;
            }
        }

        public async Task<bool> DeleteMessageAsync(int messageId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    Message message = await ctx.Messages.FirstAsync(m => m.Id == messageId);
                    message.Content = null;
                    message.HasImage = false;
                    ctx.Messages.Update(message);
                    await ctx.SaveChangesAsync();
                    return true;
                }
                catch (Exception e)
                {
                    return false;
                }
            }
        }

        public List<UserShortVersionWithMessage> GetLastMessagesForUser(int userId, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return GetLastMessagesForUserWithDbContext(ctx, userId, offset);
            }
        }

        public List<UserShortVersionWithMessage> GetLastMessagesForUserWithDbContext(ShapeAppDbContext ctx, int userId, int offset)
        {
            List<Message> userMessages = ctx.Messages.Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .OrderByDescending(m => m.TimeStamp).ToList();
                
                List<int> messagedUserIds = new List<int>();
                List<int> lastMessageIds = new List<int>();
                if (!userMessages.Any() || offset < 0)
                    return new List<UserShortVersionWithMessage>();

                foreach (var message in userMessages)
                {
                    if (message.SenderId == userId)
                    {
                        if (!messagedUserIds.Contains(message.ReceiverId))
                        {
                            messagedUserIds.Add(message.ReceiverId);
                            lastMessageIds.Add(message.Id);
                        }
                    }
                    else
                    {
                        if (!messagedUserIds.Contains(message.SenderId))
                        {
                            messagedUserIds.Add(message.SenderId);
                            lastMessageIds.Add(message.Id);
                        }
                    }
                }
                
                if (offset >= lastMessageIds.Count)
                    return new List<UserShortVersionWithMessage>();

                Console.WriteLine("Hello");
                List<UserShortVersionWithMessage> lastMessages = new List<UserShortVersionWithMessage>();
                for (int i = offset; i < offset + 10; i++)
                {
                    if (i >= lastMessageIds.Count)
                        break;

                    Console.WriteLine("i is " + i);
                    Message messageDb = userMessages.First(m => m.Id == lastMessageIds[i]);

                    string username = ctx.Users.First(u => u.Id == messagedUserIds[i]).Name;
                    MessageSocketsModel message = new MessageSocketsModel
                    {
                        Content = messageDb.Content,
                        HasImage = messageDb.HasImage,
                        Id = messageDb.Id,
                        SenderId = messageDb.SenderId,
                        ReceiverId = messageDb.ReceiverId,
                        TimeStamp = messageDb.TimeStamp
                    };
                    lastMessages.Add(new UserShortVersionWithMessage
                    {
                        UserId = messagedUserIds[i],
                        UserFullName = username,
                        Message = message
                    });
                }

                return lastMessages;
        }

        public List<MessageSocketsModel> GetConversationForUsers(int firstUserId, int secondUserId, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<Message> usersConversation = ctx.Messages.Where(m =>
                        m.SenderId == firstUserId && m.ReceiverId == secondUserId
                        || m.SenderId == secondUserId && m.ReceiverId == firstUserId)
                    .OrderByDescending(m => m.TimeStamp).ToList();

                if (offset >= usersConversation.Count)
                    return null;

                List<MessageSocketsModel> relevantMessages = new List<MessageSocketsModel>();
                for (int i = offset; i < offset + 10; i++)
                {
                    if (i >= usersConversation.Count)
                        break;

                    relevantMessages.Add(new MessageSocketsModel
                    {
                        Content = usersConversation[i].Content,
                        HasImage = usersConversation[i].HasImage,
                        Id = usersConversation[i].Id,
                        ReceiverId = usersConversation[i].ReceiverId,
                        SenderId = usersConversation[i].SenderId,
                        TimeStamp = usersConversation[i].TimeStamp
                    });
                }

                relevantMessages.Reverse();
                return relevantMessages;
            }
        }
    }
}