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
    public class DietRepo : IDietRepo
    {
        public async Task<int> AddDietAsync(DietSocketsModelWithOwner diet)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    User owner = await ctx.Users.FirstAsync(u => u.Id == diet.Owner.UserId);
                    Diet dietDb = new Diet
                    {
                        Description = diet.Description,
                        IsPublic = diet.Global,
                        Owner = owner,
                        Title = diet.Title
                    };
                    await ctx.Diet.AddAsync(dietDb);
                    await ctx.SaveChangesAsync();
                    int createdDietId = ctx.Diet.ToList().Last().Id;

                    if (diet.Meals != null && diet.Meals.Any())
                    {
                        foreach (var meal in diet.Meals)
                        {
                            int createdMealId;
                            if (meal.Id > 0)
                            {
                                createdMealId = meal.Id;
                            }
                            else
                            {
                                await ctx.Meal.AddAsync(meal);
                                await ctx.SaveChangesAsync();
                                createdMealId = ctx.Meal.ToList().Last().Id;
                            }

                            await ctx.DietMeals.AddAsync(new DietMeal
                            {
                                MealId = createdMealId,
                                DietId = createdDietId
                            });
                            await ctx.SaveChangesAsync();
                        }
                    }

                    return createdDietId;
                }
                catch (Exception e)
                {
                    return -1;
                }
            }
        }

        public async Task<DietSocketsModelWithOwner> GetDietByIdAsync(int id)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Diet diet;
                try
                {
                    diet = await ctx.Diet.Where(d => d.Id == id).Include(d => d.Owner).FirstAsync();
                }
                catch (Exception e)
                {
                    return null;
                }

                List<Meal> meals = ctx.DietMeals.Where(dm => dm.DietId == id).Select(dm => dm.Meal).ToList();
                SearchBarUser owner = new SearchBarUser
                {
                    UserId = diet.Owner.Id,
                    FullName = diet.Owner.Name
                };
                return new DietSocketsModelWithOwner
                {
                    Description = diet.Description,
                    Global = diet.IsPublic,
                    Id = diet.Id,
                    Meals = meals,
                    Owner = owner,
                    Title = diet.Title
                };
            }
        }

        public List<DietSVWithOwner> GetPublicDiets(int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<Diet> diets = ctx.Diet.Where(d => d.IsPublic)
                    .OrderBy(d => d.Title).Include(d => d.Owner).ToList();

                if (offset >= diets.Count)
                    return null;

                List<DietSVWithOwner> publicDiets = new List<DietSVWithOwner>();
                for (int i = offset; i < offset + 10; i++)
                {
                    if (i >= diets.Count)
                        break;

                    SearchBarUser owner = new SearchBarUser
                    {
                        UserId = diets[i].Owner.Id,
                        FullName = diets[i].Owner.Name
                    };
                    publicDiets.Add(new DietSVWithOwner
                    {
                        Owner = owner,
                        Id = diets[i].Id,
                        Title = diets[i].Title,
                        Description = diets[i].Description
                    });
                }

                return publicDiets;
            }
        }

        public List<DietShortVersion> GetPrivateDietsForUser(int userId, int offset)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                List<Diet> diets = ctx.Diet.Where(d => !d.IsPublic && d.Owner.Id == userId)
                    .OrderBy(d => d.Title).Include(d => d.Owner).ToList();

                if (offset >= diets.Count)
                    return null;

                // List<string> dietTitles = new List<string>();
                // for (int i = 0; i < offset; i++)
                // {
                //     dietTitles.Add(diets[i].Title);
                // }
                List<DietShortVersion> publicDiets = new List<DietShortVersion>();
                for (int i = offset; i < offset + 10; i++)
                {
                    if (i >= diets.Count)
                        break;

                    // if(dietTitles.Contains(diets[i].Title))
                    //     continue;

                    publicDiets.Add(new DietShortVersion()
                    {
                        Id = diets[i].Id,
                        Title = diets[i].Title,
                        Description = diets[i].Description
                    });
                }

                return publicDiets;
            }
        }

        public async Task<bool> EditDietAsync(DietSocketsModel diet)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                Diet dietDb = await ctx.Diet.FirstAsync(d => d.Id == diet.Id);
                if (!string.IsNullOrEmpty(diet.Title))
                    dietDb.Title = diet.Title;
                if (!string.IsNullOrEmpty(diet.Description))
                    dietDb.Description = diet.Description;

                dietDb.IsPublic = diet.Global;

                try
                {
                    ctx.Diet.Update(dietDb);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public async Task<bool> DeleteDietAsync(int dietId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                try
                {
                    Diet diet = await ctx.Diet.FirstAsync(d => d.Id == dietId);
                    List<Meal> mealsForDiet = ctx.DietMeals.Where(dm => dm.DietId == dietId)
                        .Select(dm => dm.Meal).ToList();
                    foreach (var meal in mealsForDiet)
                    {
                        if (GetNumberOfDietsForMeal(meal.Id) == 1)
                            ctx.Meal.Remove(meal);
                    }

                    ctx.Diet.Remove(diet);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public async Task<int> AddMealToDietAsync(MealWithDiet meal)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                bool exists = ctx.Meal.Any(m => m.Id == meal.Id);
                int mealId;
                if (exists)
                    mealId = meal.Id;
                else
                {
                    await ctx.Meal.AddAsync(new Meal
                    {
                        Title = meal.Title,
                        Description = meal.Description,
                        Calories = meal.Calories
                    });
                    await ctx.SaveChangesAsync();
                    mealId = ctx.Meal.ToList().Last().Id;
                }

                await ctx.DietMeals.AddAsync(new DietMeal
                {
                    MealId = mealId,
                    DietId = meal.DietId
                });
                await ctx.SaveChangesAsync();

                return mealId;
            }
        }

        public async Task<bool> EditMealInDiet(MealWithDiet meal)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                int mealFrequency = GetNumberOfDietsForMeal(meal.Id);
                Meal mealDb = await ctx.Meal.FirstAsync(m => m.Id == meal.Id);
                if (mealFrequency > 1)
                {
                    await ctx.Meal.AddAsync(new Meal
                    {
                        Calories = mealDb.Calories,
                        Description = mealDb.Description,
                        Title = mealDb.Title
                    });
                    await ctx.SaveChangesAsync();
                    int mealId = ctx.Meal.ToList().Last().Id;
                    Console.WriteLine("New meal id is " + mealId);
                    var dietMeals = ctx.DietMeals.Where(dm => dm.MealId == meal.Id &&
                                                              dm.Diet.Id != meal.DietId).ToList();
                    foreach (var dietMeal in dietMeals)
                    {
                        ctx.DietMeals.Remove(dietMeal);
                        await ctx.DietMeals.AddAsync(new DietMeal
                        {
                            DietId = dietMeal.DietId,
                            MealId = mealId
                        });
                    }
                }

                if (meal.Calories >= 0)
                    mealDb.Calories = meal.Calories;
                if (!string.IsNullOrEmpty(meal.Description))
                    mealDb.Description = meal.Description;
                if (!string.IsNullOrEmpty(meal.Title))
                    mealDb.Title = meal.Title;

                try
                {
                    ctx.Meal.Update(mealDb);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        public async Task<bool> DeleteMealFromDiet(int mealId, int dietId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                int mealFrequency = GetNumberOfDietsForMeal(mealId);
                Meal mealDb = await ctx.Meal.FirstAsync(m => m.Id == mealId);
                if (mealFrequency > 1)
                {
                    await ctx.Meal.AddAsync(new Meal
                    {
                        Calories = mealDb.Calories,
                        Description = mealDb.Description,
                        Title = mealDb.Title
                    });
                    await ctx.SaveChangesAsync();
                    int newMealId = ctx.Meal.ToList().Last().Id;
                    Console.WriteLine("New meal id is " + mealId);
                    var dietMeals = ctx.DietMeals.Where(dm => dm.MealId == mealId &&
                                                              dm.Diet.Id != dietId).ToList();
                    foreach (var dietMeal in dietMeals)
                    {
                        ctx.DietMeals.Remove(dietMeal);
                        await ctx.DietMeals.AddAsync(new DietMeal
                        {
                            DietId = dietMeal.DietId,
                            MealId = newMealId
                        });
                    }
                }

                try
                {
                    ctx.Meal.Remove(mealDb);
                    await ctx.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    return false;
                }

                return true;
            }
        }

        private int GetNumberOfDietsForMeal(int mealId)
        {
            using (ShapeAppDbContext ctx = new ShapeAppDbContext())
            {
                return ctx.DietMeals.Count(dm => dm.MealId == mealId);
            }
        }
    }
}