using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;
using MealPlanner.Data.Models;
using Dapper;
using System.Linq;

namespace MealPlanner.Data.Repositories.Dapper
{
    public class TagRepository : ITagRepository
    {
        private readonly string connectionString;

        public TagRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<Tag>> All()
        {
            var query = $"select Id, Value from Tags";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Tag>(query);
            }
        }

        public async Task<bool> Delete(Tag item)
        {
            var query = $"delete from Tags where id=@id";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.ExecuteAsync(query, new { id = item.Id }) == 1;
            }
        }

        public async Task<IEnumerable<Tag>> FindStartingWith(string startWith)
        {
            var query = $"select Id, Value from Tags where Value like @start+'%'";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Tag>(query, new { start = startWith });
            }
        }

        public async Task<IEnumerable<Tag>> ForMeal(Meal meal)
        {
            var query = $"select Id, Value from Tags t inner join TagsOfMeals tom on tom.TagId = t.Id where tom.MealId=@mealId ";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Tag>(query, new { mealId = meal.Id });
            }
        }

        public async Task<bool> Save(Tag item)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {   
                if (!item.Id.HasValue)
                {
                    connection.Open();
                    var queryInsert = @"insert into Tags(value) values (@value);
                                        select SCOPE_IDENTITY();";
                    var result = await connection.QueryAsync<int>(queryInsert, new { value = item.Value });
                    if (result.Any())
                    {
                        item.Id = result.FirstOrDefault();
                    }
                 }
            }
            return true;
        }
    }
}
