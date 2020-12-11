using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SEP3_T3.Persistance;
using SEP3_Tier3.Core;
using SEP3_Tier3.Models;

namespace SEP3_Tier3
{
    /// <summary>
    /// Main class for running the programme
    /// </summary>
    class Program
    {
        /// <summary>
        /// Main method for instantiating the factories and starting the server
        /// </summary>
        static async Task Main(string[] args)
        {
            RepositoriesFactory repoFactory = new RepositoriesFactory();
            SocketControllerFactory socketFactory = new SocketControllerFactory(repoFactory);
            ServerSocket serverSocket = new ServerSocket(socketFactory);
            //await SeedDb();
            serverSocket.Start();
        }

        /// <summary>
        /// Method for seeding the database
        /// </summary>
        private static async Task SeedDb()
        {
            var seededUserIds = await SeedUsers();
            SeedUserImages(seededUserIds);
            await SeedUserActions(seededUserIds);
            var seededPostIds = await SeedPosts(seededUserIds);
            SeedPostImages(seededPostIds);
            await SeedPostInteractions(seededPostIds, seededUserIds);
            await SeedTrainings(seededUserIds);
            await SeedDiets(seededUserIds);
            await SeedChat(seededUserIds);
        }

        /// <summary>
        /// Method for seeding the database users
        /// </summary>
        /// <returns>the ids of the seeded users</returns>
        private static async Task<int[]> SeedUsers()
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                for (int i = 0; i < 10; i++)
                {
                    User user = new User
                    {
                        City = "Horsens",
                        Description = "Description for user " + i,
                        Email = "user" + i + "@email.com",
                        Name = "Boring User" + i,
                        Password = "123456",
                        Score = i * 10 % 40
                    };
                    if (i % 3 == 0 && i != 0) //gyms at indexes 3, 6, 9
                    {
                        Address address = new Address
                        {
                            Street = "Gym street",
                            Number = i.ToString(),
                        };
                        user.Address = address;
                    }

                    await ctx.Users.AddAsync(user);
                    await ctx.SaveChangesAsync();
                }

                await ctx.Administrators.AddAsync(new Administrator
                {
                    Email = "admin@admin.com",
                    Password = "admin"
                });
                await ctx.SaveChangesAsync();
                var list = ctx.Users.OrderByDescending(u => u.Id).Select(u => u.Id).Take(10).ToList();
                list.Reverse();
                foreach (var i in list)
                {
                    Console.WriteLine("Added user with id " + i);
                }
                return list.ToArray();
            }
        }

        /// <summary>
        /// Method for seeding the user's images
        /// </summary>
        /// <param name="seededUserIds">the seeded users ids</param>
        private static void SeedUserImages(int[] seededUserIds)
        {
            try
            {
                string FILE_PATH = ImagesUtil.FILE_PATH;
                byte[] readDefaultAvatar = File.ReadAllBytes($"{FILE_PATH}/Users/defaultAvatar.jpg");
                byte[] readDefaultBg = File.ReadAllBytes($"{FILE_PATH}/Users/defaultBg.jpg");

                foreach (var userId in seededUserIds)
                {
                    ImagesUtil.WriteImageToPath(readDefaultAvatar, $"{FILE_PATH}/Users/{userId}", "/avatar.jpg");
                    ImagesUtil.WriteImageToPath(readDefaultBg, $"{FILE_PATH}/Users/{userId}", "/background.jpg");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Default avatar not found");
            }
        }

        /// <summary>
        /// Method for seeding the user's user actions
        /// </summary>
        /// <param name="seededUserIds">the seeded users ids</param>
        private static async Task SeedUserActions(int[] seededUserIds)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                //friendships
                //(0,1), (0,2), (0,5), (1,2), (1,4), (1,8)
                //((2,5), (2,7), (4,5), (4,7), (4,8), (7,8)
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[0],
                    SecondUserId = seededUserIds[1]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[0],
                    SecondUserId = seededUserIds[2]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[0],
                    SecondUserId = seededUserIds[5]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[1],
                    SecondUserId = seededUserIds[2]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[1],
                    SecondUserId = seededUserIds[4]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[1],
                    SecondUserId = seededUserIds[8]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[2],
                    SecondUserId = seededUserIds[5]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[2],
                    SecondUserId = seededUserIds[7]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[4],
                    SecondUserId = seededUserIds[5]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[4],
                    SecondUserId = seededUserIds[7]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[4],
                    SecondUserId = seededUserIds[8]
                });
                await ctx.Friendships.AddAsync(new Friendship
                {
                    FirstUserId = seededUserIds[7],
                    SecondUserId = seededUserIds[8]
                });

                //friend requests and notifications
                //(0,7), (1,5), (2,4)
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[0],
                    ReceiverId = seededUserIds[7],
                    IsFriendRequest = true
                });
                await ctx.Notifications.AddAsync(new Notification
                {
                    SenderId = seededUserIds[0],
                    ReceiverId = seededUserIds[7],
                    NotificationType = ActionType.USER_FRIEND_REQUEST_SEND.ToString()
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[1],
                    ReceiverId = seededUserIds[5],
                    IsFriendRequest = true
                });
                await ctx.Notifications.AddAsync(new Notification
                {
                    SenderId = seededUserIds[1],
                    ReceiverId = seededUserIds[5],
                    NotificationType = ActionType.USER_FRIEND_REQUEST_SEND.ToString()
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[2],
                    ReceiverId = seededUserIds[4],
                    IsFriendRequest = true
                });
                await ctx.Notifications.AddAsync(new Notification
                {
                    SenderId = seededUserIds[2],
                    ReceiverId = seededUserIds[4],
                    NotificationType = ActionType.USER_FRIEND_REQUEST_SEND.ToString()
                });

                //reports
                //(7,0), (5,1), (8,0), (6,0), (3,1), (8,2), (5,3), (4,6)
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[7],
                    ReceiverId = seededUserIds[0],
                    IsReport = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[5],
                    ReceiverId = seededUserIds[1],
                    IsReport = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[8],
                    ReceiverId = seededUserIds[0],
                    IsReport = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[6],
                    ReceiverId = seededUserIds[0],
                    IsReport = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[3],
                    ReceiverId = seededUserIds[1],
                    IsReport = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[8],
                    ReceiverId = seededUserIds[2],
                    IsReport = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[5],
                    ReceiverId = seededUserIds[3],
                    IsReport = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[4],
                    ReceiverId = seededUserIds[6],
                    IsReport = true
                });

                //share trainings and diets
                //(0,1), (0,2), (1,2), (1,4), (2,1), (4,5), (4,7), (5,4), (7,8), (8,7)
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[0],
                    ReceiverId = seededUserIds[1],
                    IsShareTrainings = true,
                    IsShareDiets = true,
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[0],
                    ReceiverId = seededUserIds[2],
                    IsShareTrainings = true,
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[1],
                    ReceiverId = seededUserIds[2],
                    IsShareTrainings = true,
                    IsShareDiets = true,
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[1],
                    ReceiverId = seededUserIds[4],
                    IsShareDiets = true,
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[2],
                    ReceiverId = seededUserIds[1],
                    IsShareTrainings = true,
                    IsShareDiets = true,
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[4],
                    ReceiverId = seededUserIds[5],
                    IsShareTrainings = true,
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[4],
                    ReceiverId = seededUserIds[7],
                    IsShareTrainings = true,
                    IsShareDiets = true,
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[5],
                    ReceiverId = seededUserIds[4],
                    IsShareDiets = true,
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[7],
                    ReceiverId = seededUserIds[8],
                    IsShareTrainings = true,
                    IsShareDiets = true,
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[8],
                    ReceiverId = seededUserIds[7],
                    IsShareTrainings = true,
                    IsShareDiets = true,
                });

                //follow page
                //(1,3), (2,3), (7,3), (0,6), (5,6), (7,6), (1,9), (2,9), (4,9), (8,9)
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[1],
                    ReceiverId = seededUserIds[3],
                    IsFollowPage = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[2],
                    ReceiverId = seededUserIds[3],
                    IsFollowPage = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[7],
                    ReceiverId = seededUserIds[3],
                    IsFollowPage = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[0],
                    ReceiverId = seededUserIds[6],
                    IsFollowPage = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[5],
                    ReceiverId = seededUserIds[6],
                    IsFollowPage = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[7],
                    ReceiverId = seededUserIds[6],
                    IsFollowPage = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[1],
                    ReceiverId = seededUserIds[9],
                    IsFollowPage = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[2],
                    ReceiverId = seededUserIds[9],
                    IsFollowPage = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[4],
                    ReceiverId = seededUserIds[9],
                    IsFollowPage = true
                });
                await ctx.UserActions.AddAsync(new UserAction
                {
                    SenderId = seededUserIds[8],
                    ReceiverId = seededUserIds[9],
                    IsFollowPage = true
                });

                //page ratings
                //(1,3), (2,3), (0,6), (5,6), (4,9), (8,9)
                await ctx.PageRatings.AddAsync(new PageRating
                {
                    UserId = seededUserIds[1],
                    PageId = seededUserIds[3],
                    Rating = 4
                });
                await ctx.PageRatings.AddAsync(new PageRating
                {
                    UserId = seededUserIds[2],
                    PageId = seededUserIds[3],
                    Rating = 5
                });
                await ctx.PageRatings.AddAsync(new PageRating
                {
                    UserId = seededUserIds[0],
                    PageId = seededUserIds[6],
                    Rating = 3
                });
                await ctx.PageRatings.AddAsync(new PageRating
                {
                    UserId = seededUserIds[5],
                    PageId = seededUserIds[6],
                    Rating = 5
                });
                await ctx.PageRatings.AddAsync(new PageRating
                {
                    UserId = seededUserIds[4],
                    PageId = seededUserIds[9],
                    Rating = 3
                });
                await ctx.PageRatings.AddAsync(new PageRating
                {
                    UserId = seededUserIds[8],
                    PageId = seededUserIds[9],
                    Rating = 4
                });

                await ctx.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Method for seeding the user's posts
        /// </summary>
        /// <param name="seededUserIds">the seeded users ids</param>
        private static async Task<int[]> SeedPosts(int[] seededUserIds)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                //posts with images: 2, 4, 5, 7, 8, 11, 12, 15, 16, 17, 20, 21, 23, 24
                List<int> postIdIndexesWithImages = new List<int> {2, 4, 5, 7, 8, 11, 12, 15, 16, 17, 20, 21, 23, 24};
                int imageOwnerId = 0;
                int postOwnerId = 0;
                for (int i = 0; i < 25; i++)
                {
                    if (postIdIndexesWithImages.Contains(i))
                    {
                        imageOwnerId = imageOwnerId % 10;
                        await ctx.Posts.AddAsync(new Post
                        {
                            Title = GetRandomString() + " with image " + i,
                            Content = "This is an exciting fitness post with image",
                            HasImage = true,
                            Owner = ctx.Users.First(u => u.Id == seededUserIds[imageOwnerId]),
                            TimeStamp = GetRandomDateTime(DateTime.Today.AddDays(-10))
                        });
                        imageOwnerId++;
                    }
                    else
                    {
                        postOwnerId = postOwnerId % 10;
                        await ctx.Posts.AddAsync(new Post
                        {
                            Title = GetRandomString() + " " + i,
                            Content = "This is an interesting fitness post",
                            HasImage = false,
                            Owner = ctx.Users.First(u => u.Id == seededUserIds[postOwnerId]),
                            TimeStamp = GetRandomDateTime(DateTime.Today.AddDays(-10))
                        });
                        postOwnerId++;
                    }

                    await ctx.SaveChangesAsync();
                }

                await ctx.SaveChangesAsync();
                var list = ctx.Posts.OrderByDescending(p => p.Id).Select(p => p.Id).Take(25).ToList();
                foreach (var i in list)
                {
                    Console.WriteLine("Added post with id " + i);
                }

                list.Reverse();
                return list.ToArray();
            }
        }

        private static DateTime GetRandomDateTime(DateTime minDate)
        {
            var random = new Random();
            int randomDate = random.Next(9) + 1;
            DateTime updatedDate = minDate.AddDays(randomDate);
            return new DateTime(updatedDate.Year, updatedDate.Month, updatedDate.Day, updatedDate.Hour + randomDate, updatedDate.Minute, updatedDate.Second);
        }

        /// <summary>
        /// Method for seeding the posts' images
        /// </summary>
        /// <param name="seededPostIds">the seeded posts ids</param>
        private static void SeedPostImages(int[] seededPostIds)
        {
            List<int> postIdIndexesWithImages = new List<int> {2, 4, 5, 7, 8, 11, 12, 15, 16, 17, 20, 21, 23, 24};
            string FILE_PATH = ImagesUtil.FILE_PATH;
            byte[] readDefaultPostImage = File.ReadAllBytes($"{FILE_PATH}/Posts/defaultPostImage.jpg");
            foreach (var postIdIndexesWithImage in postIdIndexesWithImages)
            {
                try
                {
                    ImagesUtil.WriteImageToPath(readDefaultPostImage, $"{FILE_PATH}/Posts", $"/{seededPostIds[postIdIndexesWithImage]}.jpg");
                }
                catch (Exception e)
                {
                    Console.WriteLine("Default image could not be written");
                }
            }
        }

        /// <summary>
        /// Method for seeding the seeded user's interaction with the seeded posts
        /// </summary>
        /// <param name="seededUserIds">the seeded users ids</param>
        /// <param name="seededPostIds">the seeded posts ids</param>
        private static async Task SeedPostInteractions(int[] seededPostIds, int[] seededUserIds)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Console.WriteLine("Seeding post interactions");
                Random random = new Random();
                //post actions
                foreach (var postId in seededPostIds)
                {
                    int numberOfLikes = random.Next(5) + 5; //between 5 and 10, without 10
                    int numberOfReports = seededUserIds.Length - numberOfLikes - 1; //between 0 and 4
                    
                    for (int i = 4; i < numberOfLikes; i++)
                    {
                        int userIdForLike = i;
                        await ctx.PostActions.AddAsync(new PostAction
                        {
                            PostId = postId,
                            IsLike = true,
                            UserId = seededUserIds[userIdForLike]
                        });
                    }
                    
                    for (int i = 0; i < numberOfReports; i++)
                    {
                        int userIdForReport = i;
                       
                        await ctx.PostActions.AddAsync(new PostAction
                        {
                            PostId = postId,
                            IsReport = true,
                            UserId = seededUserIds[userIdForReport]
                        });
                    }
                }

                await ctx.SaveChangesAsync();
                
                //comments
                foreach (var postId in seededPostIds)
                {
                    Post post = await ctx.Posts.Where(p => p.Id == postId)
                        .Include(p => p.Comments).FirstAsync();
                    
                    int numberOfComments = random.Next(5) + 1;
                    for (int i = 0; i < numberOfComments; i++)
                    {
                        int userIdIndexForComment = i * 3 % seededUserIds.Length;;
                        Comment comment = new Comment
                        {
                            Content = GetRandomString(),
                            Owner = ctx.Users.First(u => u.Id == seededUserIds[userIdIndexForComment]),
                            TimeStamp = post.TimeStamp.AddMinutes(random.Next(20) + 1)
                        };
                        post.Comments.Add(comment);
                        await ctx.Comment.AddAsync(comment);
                        ctx.Posts.Update(post);
                    }
                    await ctx.SaveChangesAsync();
                }

                await ctx.SaveChangesAsync();
            }
        }

        private static string GetRandomString()
        {
            Random random = new Random();
            string[] adjectives = {"Cool ", "Nice ", "Interesting ", "Amazing ", "Fascinating ", "Wonderful ", "Dumb "};
            string[] nouns = {"Post", "Training", "Diet", "Idea", "Exercise", "Meal", "Comment", "Thing"};
            return adjectives[random.Next(adjectives.Length)] + nouns[random.Next(nouns.Length)];
        }

        /// <summary>
        /// Method for seeding the user's trainings
        /// </summary>
        /// <param name="seededUserIds">the seeded users ids</param>
        private static async Task SeedTrainings(int[] seededUserIds)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Console.WriteLine("Seeding trainings");
                Random random = new Random();
                string[] trainingTypes = {"Muscle Gain", "Weight loss", "Endurance", "Strength"};
                foreach (var userId in seededUserIds)
                {
                    int userNumberOfTrainings = random.Next(3) + 1;
                    for (int i = 0; i < userNumberOfTrainings; i++)
                    {
                        await ctx.Training.AddAsync(new Training
                        {
                            Duration = random.Next(60) + 40,
                            IsPublic = i == 1,
                            IsCompleted = random.Next(2) == 0,
                            TimeStamp = GetRandomTrainingTime(DateTime.Today.AddDays(-5), i),
                            Title = GetRandomString(),
                            Owner = ctx.Users.First(u => u.Id == userId),
                            Type = trainingTypes[random.Next(trainingTypes.Length)]
                        });
                        await ctx.SaveChangesAsync();
                        int createdTrainingId = ctx.Training.ToList().Last().Id;

                        int trainingNumberOfExercises = random.Next(3);
                        for (int j = 0; j < trainingNumberOfExercises; j++)
                        {
                            await ctx.Exercise.AddAsync(new Exercise
                            {
                                Title = GetRandomString(),
                                Description = "Exercise description: " + GetRandomString()
                            });
                            await ctx.SaveChangesAsync();
                            int createdExerciseId = ctx.Exercise.ToList().Last().Id;
                            await ctx.TrainingExercises.AddAsync(new TrainingExercise
                            {
                                TrainingId = createdTrainingId,
                                ExerciseId = createdExerciseId
                            });
                            await ctx.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        private static DateTime GetRandomTrainingTime(DateTime minDate, int index)
        {
            var random = new Random();
            int randomDate = random.Next(9) + 1;
            DateTime updatedDate = minDate.AddDays(randomDate);
            int hour = 10 + index * 2;
            return new DateTime(updatedDate.Year, updatedDate.Month, updatedDate.Day, hour + randomDate, updatedDate.Minute, updatedDate.Second);
        }

        /// <summary>
        /// Method for seeding the user's diets
        /// </summary>
        /// <param name="seededUserIds">the seeded users ids</param>
        private static async Task SeedDiets(int[] seededUserIds)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Random random = new Random();
                foreach (var userId in seededUserIds)
                {
                    int userNumberOfDiets = random.Next(3) + 1;
                    for (int i = 0; i < userNumberOfDiets; i++)
                    {
                        await ctx.Diet.AddAsync(new Diet
                        {
                            IsPublic = i == 1,
                            Description = "Diet description: " + GetRandomString(),
                            Title = GetRandomString(),
                            Owner = ctx.Users.First(u => u.Id == userId),
                        });
                        await ctx.SaveChangesAsync();
                        int createdDietId = ctx.Diet.ToList().Last().Id;

                        int dietNumberOfMeals = random.Next(3);
                        for (int j = 0; j < dietNumberOfMeals; j++)
                        {
                            await ctx.Meal.AddAsync(new Meal
                            {
                                Title = GetRandomString(),
                                Description = "Meal description: " + GetRandomString(),
                                Calories = (random.Next(12) + 1) * 100
                            });
                            await ctx.SaveChangesAsync();
                            int createdMealId = ctx.Meal.ToList().Last().Id;
                            await ctx.DietMeals.AddAsync(new DietMeal
                            {
                                DietId = createdDietId,
                                MealId = createdMealId
                            });
                            await ctx.SaveChangesAsync();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Method for seeding the user's user chat messages
        /// </summary>
        /// <param name="seededUserIds">the seeded users ids</param>
        private static async Task SeedChat(int[] seededUserIds)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                for (int i = 0; i < seededUserIds.Length - 1; i++)
                {
                    for (int j = i + 1; j < seededUserIds.Length; j++)
                    {
                        await ctx.Messages.AddAsync(new Message
                        {
                            SenderId = seededUserIds[i],
                            ReceiverId = seededUserIds[j],
                            Content = GetRandomString(),
                            HasImage = false,
                            TimeStamp = GetRandomDateTime(DateTime.Today.AddDays(-10))
                        });
                        await ctx.Messages.AddAsync(new Message
                        {
                            SenderId = seededUserIds[j],
                            ReceiverId = seededUserIds[i],
                            Content = GetRandomString(),
                            HasImage = false,
                            TimeStamp = GetRandomDateTime(DateTime.Today.AddDays(-10))
                        });
                    }
                }

                await ctx.SaveChangesAsync();
            }
        }
    }
}