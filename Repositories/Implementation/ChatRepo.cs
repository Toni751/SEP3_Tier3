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
    public class ChatRepo : IChatRepo
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
                List<Message> userMessages = ctx.Messages.Where(m => m.SenderId == userId || m.ReceiverId == userId)
                    .OrderByDescending(m => m.TimeStamp).ToList();
                foreach (var userMessage in userMessages)
                {
                    Console.WriteLine("Message id is " + userMessage.Id + " with receiver " + userMessage.ReceiverId);
                }

                Console.WriteLine("Total number of messages for user " + userId + " is " + userMessages.Count);

                List<int> messagedUserIds = new List<int>();
                List<int> lastMessageIds = new List<int>();
                if (!userMessages.Any())
                    return null;

                foreach (var message in userMessages)
                {
                    Console.WriteLine("Message id is " + message.Id);
                    if (message.SenderId == userId)
                    {
                        if (!messagedUserIds.Contains(message.ReceiverId))
                        {
                            Console.WriteLine("Here 1 with message " + message.Id);
                            messagedUserIds.Add(message.ReceiverId);
                            lastMessageIds.Add(message.Id);
                        }
                    }
                    else
                    {
                        Console.WriteLine("Attempting with message " + message.Id + " with sender " + message.SenderId);
                        if (!messagedUserIds.Contains(message.SenderId))
                        {
                            Console.WriteLine("Here 2 with message " + message.Id);
                            messagedUserIds.Add(message.SenderId);
                            lastMessageIds.Add(message.Id);
                        }
                    }
                }

                // Console.WriteLine("Messaged user ids count is " + messagedUserIds.Count);
                // Console.WriteLine("User last messaged user ids: " + messagedUserIds[0] + " " + messagedUserIds[1]);
                // Console.WriteLine("Last messages length is " + lastMessageIds.Count);
                if (offset >= lastMessageIds.Count)
                    return null;

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