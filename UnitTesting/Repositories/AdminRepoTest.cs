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
    /// Class for unit testing the admin repository with the specified data set
    /// </summary>
    public class AdminRepoTest
    {
        [Theory, MemberData("TestData")]
        public void GetReportedUsers_WithLimitAndOffset_RetrievesExpectedUsersInAllCases(int limit, int offset, List<UserShortVersion> expected)
        {
            //arrange
            using (DummyDbContext ctx = new DummyDbContext())
            {
                IAdminRepoTest adminRepo = new AdminRepo();

                //act
                List<UserShortVersion> actual = adminRepo.GetAdminUsersWithDbContextAsync(ctx, limit, offset);

                //assert
                Assert.Equal(expected.Count, actual.Count);
                for (int i = 0; i < actual.Count; i++)
                {
                    Assert.Equal(expected[i].UserId, actual[i].UserId);
                    Assert.Equal(expected[i].UserFullName, actual[i].UserFullName);
                }
            }
        }

        public static IEnumerable<object[]> TestData {
            get {
                using (DummyDbContext ctx = new DummyDbContext()) {
                    List<User> dbUsers = ctx.Users.ToList();

                    //this list contains the all reported users, in descending order by the total number of reports
                    //user1 has 3 reports, user2 has 2 reports, user 3 has 1 report and user4 has 0 reports
                    var reportedUsersListWithOffset0 = new List<UserShortVersion>();
                    foreach (var dbUser in dbUsers) {
                        reportedUsersListWithOffset0.Add(new UserShortVersion {
                            UserId = dbUser.Id,
                            UserFullName = dbUser.Name
                        });
                    }

                    //this list contains the 4th least reported user, i.e. User4
                    var reportedUsersListWithOffset3 = new List<UserShortVersion>() {reportedUsersListWithOffset0[3]};
                    
                    //this list contains the 2nd, 3rd and 4th least reported users, i.e. User2, User3 and User4
                    var reportedUsersListWithOffset1 = new List<UserShortVersion>() {reportedUsersListWithOffset0[1], reportedUsersListWithOffset0[2], reportedUsersListWithOffset0[3]};

                    //this list contains the most reported user, i.e. User1
                    var reportedUsersListWithLimit1 = new List<UserShortVersion>() {reportedUsersListWithOffset0[0]};

                    //this list contains the 3 most reported users, i.e. User1, User2 and User3
                    var reportedUsersListWithLimit3 = new List<UserShortVersion>() {reportedUsersListWithOffset0[0], reportedUsersListWithOffset0[1], reportedUsersListWithOffset0[2]};

                    //this list contains the 2 most reported users with and offset of 2, i.e. User3 and User4
                    var reportedUsersListWithLimit2AndOffset2 = new List<UserShortVersion>() {reportedUsersListWithOffset0[2], reportedUsersListWithOffset0[3]};

                    return new[] {
                        //tests for limit >= total number of reported users(4) and boundary offset values
                        new object[] {5, 0, reportedUsersListWithOffset0}, //all reported users in descending order
                        new object[] {5, 1, reportedUsersListWithOffset1},
                        new object[] {5, 3, reportedUsersListWithOffset3},
                        new object[] {5, 4, new List<UserShortVersion> { }},
                        new object[] {5, -1, new List<UserShortVersion> { }},

                        //tests for no offset(0) and boundary limit values
                        new object[] {0, 0, new List<UserShortVersion> { }},
                        new object[] {1, 0, reportedUsersListWithLimit1},
                        new object[] {3, 0, reportedUsersListWithLimit3},
                        new object[] {4, 0, reportedUsersListWithOffset0}, //all reported users in descending order
                        new object[] {-1, 0, new List<UserShortVersion> { }},

                        //tests for boundary cases for both limit and offset + 1 random in-between value for both
                        new object[] {-1, -1, new List<UserShortVersion> { }},
                        new object[] {0, -1, new List<UserShortVersion> { }},
                        new object[] {2, 2, reportedUsersListWithLimit2AndOffset2}, //random middle value
                        new object[] {3, 4, new List<UserShortVersion> { }},
                    };
                }
            }
        }
    }
}