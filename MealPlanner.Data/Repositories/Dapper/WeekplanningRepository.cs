using Dapper;
using MealPlanner.Data.Models;
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
                connection.Open();
                var query = "select id, week, year, Sunday, Monday, Tuesday, Wednesday, Thursday, Friday, Saturday from WeekPlans where year=@year and week=@week";
                var result = (await connection.QueryAsync<Weekplanning>(query, new { year = year, week = week })).FirstOrDefault();
                if (result != null)
                {
                    await this.MealRepository.PairMealsToDay(result.Days); 
                }
                return result;
            }
        }

        public async Task<bool> Save(Weekplanning item)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
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

                var queryUpdate = $"update WeekPlans set Sunday=@meal1, Monday=@meal2, Tuesday=@meal3, Wednesday=@meal4,Thursday=@meal5,Friday=@meal6, Saturday=meal7 where year=@year and week=@week";

                var updateCommand = new SqlCommand(queryUpdate, connection);

                updateCommand.Parameters.Add(new SqlParameter("year", item.Year));
                updateCommand.Parameters.Add(new SqlParameter("week", item.Week));
                updateCommand.Parameters.Add(new SqlParameter("meal1", item.Sunday));
                updateCommand.Parameters.Add(new SqlParameter("meal2", item.Monday));
                updateCommand.Parameters.Add(new SqlParameter("meal3", item.Tuesday));
                updateCommand.Parameters.Add(new SqlParameter("meal4", item.Wednesday));
                updateCommand.Parameters.Add(new SqlParameter("meal5", item.Thursday));
                updateCommand.Parameters.Add(new SqlParameter("meal6", item.Friday));
                updateCommand.Parameters.Add(new SqlParameter("meal7", item.Saturday));

            }
            return true;
        }
    }
}
