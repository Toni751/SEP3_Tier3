using System.Collections.Generic;
using System.Linq;
using SEP3_T3.Persistance;
using SEP3_Tier3.Models;
using SEP3_Tier3.Repositories.Implementation;
using SEP3_Tier3.Repositories.UnitTestInterfaces;
using Xunit;

namespace SEP3_Tier3.UnitTesting.Repositories
{
    /// <summary>
    /// Class for unit testing the chat repository with the specified data set
    /// </summary>
    public class ChatRepoTest
    {
        [Theory, MemberData("TestData")]
        public void GetLastMessagesForUser_WithOffset_RetrievesExpectedUsersWithMessages_InAllCases(int userId, int offset,
            List<UserShortVersionWithMessage> expected)
        {
            //arrange
            using (DummyDbContext ctx = new DummyDbContext())
            {
                IChatRepoTest chatRepo = new ChatRepo();

                //act
                List<UserShortVersionWithMessage> actual = chatRepo.GetLastMessagesForUserWithDbContext(ctx, userId, offset);

                //assert
                Assert.Equal(expected.Count, actual.Count);
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.Equal(expected[i].UserId, actual[i].UserId);
                    Assert.Equal(expected[i].UserFullName, actual[i].UserFullName);
                    Assert.Equal(expected[i].Message.Id, actual[i].Message.Id);
                }
            }
        }

        public static IEnumerable<object[]> TestData {
            get {
                using (DummyDbContext ctx = new DummyDbContext()) {
                    List<User> dbUsers = ctx.Users.ToList();
                    
                    List<Message> dbMessages = ctx.Messages.ToList();
                    MessageSocketsModel lastMessageUsers1And2 = new MessageSocketsModel {Id = dbMessages[1].Id,};
                    MessageSocketsModel lastMessageUsers1And3 = new MessageSocketsModel {Id = dbMessages[2].Id,};
                    MessageSocketsModel lastMessageUsers1And4 = new MessageSocketsModel {Id = dbMessages[3].Id,};
                    MessageSocketsModel lastMessageUsers2And3 = new MessageSocketsModel(); //no message
                    MessageSocketsModel lastMessageUsers2And4 = new MessageSocketsModel {Id = dbMessages[5].Id,};
                    MessageSocketsModel lastMessageUsers3And4 = new MessageSocketsModel {Id = dbMessages[6].Id,};
                    
                    List<UserShortVersionWithMessage> user1LastMessages = new List<UserShortVersionWithMessage>();
                    user1LastMessages.Add(new UserShortVersionWithMessage {
                        UserId = dbUsers[3].Id,
                        UserFullName = dbUsers[3].Name,
                        Message = lastMessageUsers1And4
                    });
                    user1LastMessages.Add(new UserShortVersionWithMessage {
                        UserId = dbUsers[2].Id,
                        UserFullName = dbUsers[2].Name,
                        Message = lastMessageUsers1And3
                    });
                    user1LastMessages.Add(new UserShortVersionWithMessage {
                        UserId = dbUsers[1].Id,
                        UserFullName = dbUsers[1].Name,
                        Message = lastMessageUsers1And2
                    });
                    
                    List<UserShortVersionWithMessage> user4LastMessages = new List<UserShortVersionWithMessage>();
                    user4LastMessages.Add(new UserShortVersionWithMessage {
                        UserId = dbUsers[2].Id,
                        UserFullName = dbUsers[2].Name,
                        Message = lastMessageUsers3And4
                    });
                    user4LastMessages.Add(new UserShortVersionWithMessage {
                        UserId = dbUsers[1].Id,
                        UserFullName = dbUsers[1].Name,
                        Message = lastMessageUsers2And4
                    });
                    user4LastMessages.Add(new UserShortVersionWithMessage {
                        UserId = dbUsers[0].Id,
                        UserFullName = dbUsers[0].Name,
                        Message = lastMessageUsers1And4
                    });
                    
                    var user1LastMessagesWithOffset2 = new List<UserShortVersionWithMessage>() {user1LastMessages[2]};
                    
                    List<UserShortVersionWithMessage> user3LastMessagesWithOffset1 = new List<UserShortVersionWithMessage>();
                    user3LastMessagesWithOffset1.Add(new UserShortVersionWithMessage {
                        UserId = dbUsers[0].Id,
                        UserFullName = dbUsers[0].Name,
                        Message = lastMessageUsers1And3
                    });

                    return new[] {
                        //tests for fixed offset(0) and boundary values for userIds (0, 1, 4, 5)
                         new object[] {1, 0, user1LastMessages},
                         new object[] {4, 0, user4LastMessages},
                         new object[] {0, 0, new List<UserShortVersionWithMessage> { }},
                         new object[] {5, 0, new List<UserShortVersionWithMessage> { }},

                        //tests for fixed userId 1 (user which has 3 chats) and boundary values for offset (-1, 2, 3, 4)
                         new object[] {1, 2, user1LastMessagesWithOffset2},
                         new object[] {1, 3, new List<UserShortVersionWithMessage> { }},
                         new object[] {1, -1, new List<UserShortVersionWithMessage> { }},
                         new object[] {1, 4, new List<UserShortVersionWithMessage> { }},

                        //tests for boundary/exceptional cases for both userId and offset + 1 random in-between value
                         new object[] {0, -1, new List<UserShortVersionWithMessage> { }},
                         new object[] {5, 4, new List<UserShortVersionWithMessage> { }},
                         new object[] {5, -1, new List<UserShortVersionWithMessage> { }},
                         new object[] {3, 1, user3LastMessagesWithOffset1},
                    };
                }
            }
        }
    }
}