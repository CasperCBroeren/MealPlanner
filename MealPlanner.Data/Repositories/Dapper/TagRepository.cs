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

        public async Task<IEnumerable<Tag>> All(int groupId)
        {
            var query = $"select Id, Value from Tags where groupId = @groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
               await connection.OpenAsync();
                return await connection.QueryAsync<Tag>(query, new { groupId });
            }
        }

        public async Task<bool> Delete(Tag item, int groupId)
        {
            var query = $"delete from Tags where id=@id";
            using (var connection = new SqlConnection(this.connectionString))
            {
               await connection.OpenAsync();
                return await connection.ExecuteAsync(query, new { id = item.Id, groupId }) == 1;
            }
        }

        public async Task<Tag> Find(string tag, int groupId)
        {
            var query = $"select Id, Value from Tags where Value like @tag and groupId=@groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
               await connection.OpenAsync();
                return (await connection.QueryAsync<Tag>(query, new { groupId, tag })).FirstOrDefault();
            }
        }

        public async Task<IEnumerable<Tag>> FindStartingWith(string startWith, int groupId)
        {
            var query = $"select Id, Value from Tags where Value like @start+'%' and groupId = @groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
               await connection.OpenAsync();
                return await connection.QueryAsync<Tag>(query, new { start = startWith, groupId });
            }
        }

        public async Task<IEnumerable<Tag>> ForMeal(Meal meal)
        {
            var query = $"select Id, Value from Tags t inner join TagsOfMeals tom on tom.TagId = t.Id where tom.MealId=@mealId ";
            using (var connection = new SqlConnection(this.connectionString))
            {
               await connection.OpenAsync();
                return await connection.QueryAsync<Tag>(query, new { mealId = meal.Id });
            }
        }

        public async Task<bool> Save(Tag item, int groupId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {   
                if (!item.Id.HasValue)
                {
                   await connection.OpenAsync();
                    var queryInsert = @"insert into Tags(value, groupId) values (@value, @groupId);
                                        select SCOPE_IDENTITY();";
                    var result = await connection.QueryAsync<int>(queryInsert, new { value = item.Value, groupId });
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
