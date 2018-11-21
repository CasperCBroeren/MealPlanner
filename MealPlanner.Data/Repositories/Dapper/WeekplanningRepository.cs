using Dapper;
using MealPlanner.Data.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories.Dapper
{
    public class WeekplanningRepository : IWeekplanningRepository
    {
        private readonly IMealRepository MealRepository;
        private readonly string connectionString;

        public WeekplanningRepository(IMealRepository mealRepository, string connectionString)
        {
            this.MealRepository = mealRepository;
            this.connectionString = connectionString;
        }

        public async Task<Weekplanning> GetForWeekAndYear(int year, int week)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                try
                {
                    await connection.OpenAsync();
                    var query = "select id, week, year, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday from WeekPlans where year=@year and week=@week";
                    var result = (await connection.QueryAsync(query, new { year = year, week = week })).Select(x =>
                    {
                        var wk = new Weekplanning
                        {
                            Id = x.id,
                            Week = x.week,
                            Year = x.year,
                            Days = new List<Day>() {
                            new Day("zondag", x.Sunday),
                            new Day("maandag", x.Monday),
                            new Day("dinsdag", x.Tuesday),
                            new Day("woensdag", x.Wednesday),
                            new Day("donderdag", x.Thursday),
                            new Day("vrijdag", x.Friday),
                            new Day("zaterdag", x.Saturday) }
                        };
                        return wk;
                    }).FirstOrDefault();
                    if (result != null)
                    {
                        await this.MealRepository.PairMealsToDay(result.Days);
                    }
                    return result;
                }
                catch (Exception exc)
                {
                    throw exc;
                }
            }
        }

        public async Task<Weekplanning> Save(Weekplanning item)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                if (!item.Id.HasValue)
                {
                    var queryInsert = @"insert into WeekPlans(year, week) values (@year, @week);
                                        select SCOPE_IDENTITY();";
                    var result = await connection.QueryAsync<int>(queryInsert, new { year = item.Year, week = item.Week });
                    if (result.Any())
                    {
                        item.Id = result.FirstOrDefault();
                    }
                }

                var queryUpdate = $"update WeekPlans set Sunday=@meal1, Monday=@meal2, Tuesday=@meal3, Wednesday=@meal4,Thursday=@meal5,Friday=@meal6, Saturday=@meal7 where year=@year and week=@week";
                await connection.ExecuteAsync(queryUpdate, new
                {
                    year = item.Year,
                    week = item.Week,
                    meal1 = item.Days[0].Meal?.Id,
                    meal2 = item.Days[1].Meal?.Id,
                    meal3 = item.Days[2].Meal?.Id,
                    meal4 = item.Days[3].Meal?.Id,
                    meal5 = item.Days[4].Meal?.Id,
                    meal6 = item.Days[5].Meal?.Id,
                    meal7 = item.Days[6].Meal?.Id
                });
            }
            return item;
        }
    }
}
