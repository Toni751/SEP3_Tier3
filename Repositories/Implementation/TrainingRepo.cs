using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using SEP3_T3.Persistance;
using SEP3_Tier3.Models;

namespace SEP3_Tier3.Repositories.Implementation
{
    public class TrainingRepo : ITrainingRepo
    {
        public async Task<int> AddTrainingAsync(TrainingSocketsModelWithOwner training)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    User owner = await ctx.Users.FirstAsync(u => u.Id == training.Owner.UserId);
                    Training trainingDb = new Training
                    {
                        Title = training.Title, 
                        TimeStamp = training.TimeStamp,
                        IsCompleted = training.Completed,
                        Duration = training.Duration,
                        IsPublic = training.Global,
                        Type = training.Type,
                        Owner = owner
                    };
                    await ctx.Training.AddAsync(trainingDb);
                    await ctx.SaveChangesAsync();
                    int createdTrainingId = ctx.Training.ToList().Last().Id;
                    
                    if (training.Exercises != null && training.Exercises.Any()) {
                        foreach (var exercise in training.Exercises) {
                            int createdExerciseId;
                            if (exercise.Id > 0)
                            {
                                createdExerciseId = exercise.Id;
                            }
                            else
                            {
                                await ctx.Exercise.AddAsync(exercise);
                                await ctx.SaveChangesAsync();
                                createdExerciseId = ctx.Exercise.ToList().Last().Id;
                            }
                            
                            await ctx.TrainingExercises.AddAsync(new TrainingExercise
                            {
                                ExerciseId = createdExerciseId,
                                TrainingId = createdTrainingId
                            });
                            await ctx.SaveChangesAsync();
                        }
                    }

                    return createdTrainingId;
                }
                catch (Exception e)
                {
                    return -1;
                }
            }
        }

        public async Task<TrainingSocketsModelWithOwner> GetTrainingByIdAsync(int id)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Training training;
                try {
                    training = await ctx.Training.Where(t => t.Id == id)
                        .Include(t => t.Owner).FirstAsync();
                }
                catch (Exception e) {
                    return null;
                }
                List<Exercise> exercises = ctx.TrainingExercises.Where(te => te.TrainingId == id)
                    .Select(te => te.Exercise).ToList();
                SearchBarUser owner = new SearchBarUser
                {
                    UserId = training.Owner.Id,
                    FullName = training.Owner.Name
                };
                return new TrainingSocketsModelWithOwner
                {
                    Duration = training.Duration,
                    Exercises = exercises,
                    Id = training.Id,
                    Completed = training.IsCompleted,
                    Global = training.IsPublic,
                    Owner = owner,
                    TimeStamp = training.TimeStamp,
                    Title = training.Title,
                    Type = training.Type
                };
            }
        }

        public List<TrainingSVWithOwner> GetPublicTrainings(int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<Training> trainings = ctx.Training.Where(t => t.IsPublic)
                    .OrderByDescending(t => t.TimeStamp).Include(t => t.Owner).ToList();

                if (offset >= trainings.Count)
                    return null;
                
                List<TrainingSVWithOwner> publicTrainings = new List<TrainingSVWithOwner>();
                for (int i = offset; i < offset + 10; i++)
                {
                    if(i >= trainings.Count)
                        break;
                    
                    SearchBarUser owner = new SearchBarUser
                    {
                        UserId = trainings[i].Owner.Id,
                        FullName = trainings[i].Owner.Name
                    };
                    publicTrainings.Add(new TrainingSVWithOwner
                    {
                        Owner = owner,
                        TrainingId = trainings[i].Id,
                        TrainingTitle = trainings[i].Title
                    });
                }

                return publicTrainings;
            }
        }

        public List<TrainingShortVersion> GetPrivateTrainingsForUser(int userId, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<Training> trainings = ctx.Training.Where(t => !t.IsPublic && t.Owner.Id == userId)
                    .OrderByDescending(t => t.TimeStamp).ToList();

                if (offset >= trainings.Count)
                    return null;
                
                List<string> trainingTitles = new List<string>();
                for (int i = 0; i < offset; i++)
                {
                    trainingTitles.Add(trainings[i].Title);
                }
                List<TrainingShortVersion> publicTrainings = new List<TrainingShortVersion>();
                for (int i = offset; i < offset + 10; i++)
                {
                    if(i >= trainings.Count)
                        break;
                    
                    if(trainingTitles.Contains(trainings[i].Title))
                        continue;
                    
                    publicTrainings.Add(new TrainingSVWithOwner
                    {
                        TrainingId = trainings[i].Id,
                        TrainingTitle = trainings[i].Title
                    });
                    trainingTitles.Add(trainings[i].Title);
                }

                return publicTrainings;
            }
        }

        public List<TrainingSVWithOwner> GetTrainingsForUser(int id, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<Training> trainings = ctx.Training.Where(t => t.IsPublic || t.Owner.Id == id)
                    .OrderByDescending(t => t.TimeStamp).Include(t => t.Owner).ToList();

                if (offset >= trainings.Count)
                    return null;
                
                List<TrainingSVWithOwner> publicTrainings = new List<TrainingSVWithOwner>();
                for (int i = offset; i < offset + 10; i++)
                {
                    if(i >= trainings.Count)
                        break;
                    
                    SearchBarUser owner = new SearchBarUser
                    {
                        UserId = trainings[i].Owner.Id,
                        FullName = trainings[i].Owner.Name
                    };
                    publicTrainings.Add(new TrainingSVWithOwner
                    {
                        Owner = owner,
                        TrainingId = trainings[i].Id,
                        TrainingTitle = trainings[i].Title
                    });
                }

                return publicTrainings;
            }
        }

        public List<TrainingSVWithTime> GetTrainingsInWeekForUser(int userId, int weekNumber)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                DateTime mondayOfGivenWeek = FirstDateOfWeek(weekNumber);
                DateTime sundayOfGivenWeek = mondayOfGivenWeek.AddDays(6);
                List<Training> trainings = ctx.Training.Where(t => t.Owner.Id == userId && 
                             t.TimeStamp.CompareTo(mondayOfGivenWeek) >= 0 && t.TimeStamp.CompareTo(sundayOfGivenWeek) <= 0).ToList();
                
                List<TrainingSVWithTime> trainingsInWeek = new List<TrainingSVWithTime>();
                foreach (var training in trainings)
                {
                    trainingsInWeek.Add(new TrainingSVWithTime
                    {
                        Duration = training.Duration,
                        TimeStamp = training.TimeStamp,
                        TrainingId = training.Id,
                        TrainingTitle = training.Title
                    });
                }

                return trainingsInWeek;
            }
        }

        public List<TrainingSVWithTime> GetTrainingsTodayForUser(int userId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                DateTime today = DateTime.Today;
                List<Training> trainings = ctx.Training.Where(t => 
                    t.Owner.Id == userId && t.TimeStamp.Date.CompareTo(today) == 0).OrderBy(t => t.TimeStamp).ToList();
                
                List<TrainingSVWithTime> trainingsInWeek = new List<TrainingSVWithTime>();
                foreach (var training in trainings)
                {
                    trainingsInWeek.Add(new TrainingSVWithTime
                    {
                        Duration = training.Duration,
                        TimeStamp = training.TimeStamp,
                        TrainingId = training.Id,
                        TrainingTitle = training.Title
                    });
                }

                return trainingsInWeek;
            }
        }

        public async Task<bool> EditTrainingAsync(TrainingSocketsModel training)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Training trainingDb = await ctx.Training.FirstAsync(t => t.Id == training.Id);
                if (!string.IsNullOrEmpty(training.Title))
                    trainingDb.Title = training.Title;
                if (!string.IsNullOrEmpty(training.Type))
                    trainingDb.Type = training.Type;
                if (training.Duration > 0)
                    trainingDb.Duration = training.Duration;
                if(training.TimeStamp.Year != 1)
                    trainingDb.TimeStamp = training.TimeStamp;
                
                trainingDb.IsCompleted = training.Completed;
                trainingDb.IsPublic = training.Global;
                
                try
                {
                    ctx.Training.Update(trainingDb);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return false;
                }

                return true;
            }
        }

        public async Task<bool> DeleteTrainingAsync(int trainingId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try {
                    Training training = await ctx.Training.FirstAsync(t => t.Id == trainingId);
                    List<Exercise> exercisesForTraining = ctx.TrainingExercises.Where(te => te.TrainingId == trainingId)
                            .Select(te => te.Exercise).ToList();
                    Console.WriteLine("Training " + trainingId + " has " + exercisesForTraining.Count + " exercises");
                    foreach (var exercise in exercisesForTraining)
                    {
                        if (GetNumberOfTrainingsForExercise(exercise.Id) == 1)
                            ctx.Exercise.Remove(exercise);
                    }
                    ctx.Training.Remove(training);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return false;
                }

                return true;
            }
        }

        public async Task<int> AddExerciseToTrainingAsync(ExerciseWithTraining exercise)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                bool exists = ctx.Exercise.Any(e => e.Id == exercise.Id);
                int exerciseId;
                if (exists)
                    exerciseId = exercise.Id;
                else
                {
                    await ctx.Exercise.AddAsync(new Exercise
                    {
                        Description = exercise.Description,
                        Title = exercise.Title
                    });
                    await ctx.SaveChangesAsync();
                    exerciseId = ctx.Exercise.ToList().Last().Id;
                }
                await ctx.TrainingExercises.AddAsync(new TrainingExercise
                {
                    ExerciseId = exerciseId,
                    TrainingId = exercise.TrainingId
                });
                await ctx.SaveChangesAsync();

                return exerciseId;
            }
        }

        public async Task<bool> EditExerciseInTrainingAsync(ExerciseWithTraining exercise)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                int exerciseFrequency = GetNumberOfTrainingsForExercise(exercise.Id);
                Exercise exerciseDb = await ctx.Exercise.FirstAsync(e => e.Id == exercise.Id);
                if (exerciseFrequency > 1)
                {
                    await ctx.Exercise.AddAsync(new Exercise
                    {
                        Description = exerciseDb.Description,
                        Title = exerciseDb.Title
                    });
                    await ctx.SaveChangesAsync();
                    int exerciseId = ctx.Exercise.ToList().Last().Id;
                    Console.WriteLine("New exercise id is " + exerciseId);
                    var trainingExercises = ctx.TrainingExercises.Where(te => te.ExerciseId == exercise.Id &&
                                                                              te.TrainingId != exercise.TrainingId).ToList();
                    Console.WriteLine("Updating " + trainingExercises.Count + " training exercises");
                    foreach (var trainingExercise in trainingExercises)
                    {
                        ctx.TrainingExercises.Remove(trainingExercise);
                        await ctx.TrainingExercises.AddAsync(new TrainingExercise
                        {
                            ExerciseId = exerciseId,
                            TrainingId = trainingExercise.TrainingId
                        });
                    }
                }

                if (!string.IsNullOrEmpty(exercise.Title))
                    exerciseDb.Title = exercise.Title;
                if (!string.IsNullOrEmpty(exercise.Description))
                    exerciseDb.Description = exercise.Description;
                
                try
                {
                    ctx.Exercise.Update(exerciseDb);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return false;
                }

                return true;
            }
        }

        public async Task<bool> DeleteExerciseFromTrainingAsync(int exerciseId, int trainingId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                int exerciseFrequency = GetNumberOfTrainingsForExercise(exerciseId);
                Exercise exerciseDb = await ctx.Exercise.FirstAsync(e => e.Id == exerciseId);
                if (exerciseFrequency > 1)
                {
                    await ctx.Exercise.AddAsync(new Exercise
                    {
                        Description = exerciseDb.Description,
                        Title = exerciseDb.Title
                    });
                    await ctx.SaveChangesAsync();
                    int lastExerciseId = ctx.Exercise.ToList().Last().Id;
                    Console.WriteLine("New exercise id is " + lastExerciseId);
                    var trainingExercises = ctx.TrainingExercises.Where(te => te.ExerciseId == exerciseId &&
                                                                              te.TrainingId != trainingId).ToList();
                    Console.WriteLine("Updating " + trainingExercises.Count + " training exercises");
                    foreach (var trainingExercise in trainingExercises)
                    {
                        ctx.TrainingExercises.Remove(trainingExercise);
                        await ctx.TrainingExercises.AddAsync(new TrainingExercise
                        {
                            ExerciseId = lastExerciseId,
                            TrainingId = trainingExercise.TrainingId
                        });
                    }
                }

                try
                {
                    ctx.Exercise.Remove(exerciseDb);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e) {
                    return false;
                }

                return true;
            }
        }

        private int GetNumberOfTrainingsForExercise(int exerciseId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return ctx.TrainingExercises.Count(te => te.ExerciseId == exerciseId);
            }
        }
        
        private DateTime FirstDateOfWeek(int weekNumber)
        {
            DateTime jan1 = new DateTime(DateTime.Today.Year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekNumber;
            if (firstWeek == 1)
                weekNum -= 1;


            var result = firstThursday.AddDays(weekNum * 7);
            //this returns the monday date of the given week
            return result.AddDays(-3);
        }
    }
}