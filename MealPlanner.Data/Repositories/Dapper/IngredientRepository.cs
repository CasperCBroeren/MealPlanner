using MealPlanner.Data.Models;
using System.Collections.Generic;
using Dapper;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Linq;

namespace MealPlanner.Data.Repositories.Dapper
{
    public class IngredientRepository : IIngredientRepository
    {
        private readonly string connectionString;

        public IngredientRepository(string connectionstring)
        {
            this.connectionString = connectionstring;
        }
        public async Task<IEnumerable<Ingredient>> All(int groupId)
        {
            var query = $"select * from Ingredients where groupid=@groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Ingredient>(query, new { groupId });
            }
        }

        public async Task<bool> Delete(int groupId, Ingredient item)
        {
            var query = $"delete from Ingredients where id=@id groupId=@groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                return await connection.ExecuteAsync(query,new { id = item.Id, groupId }) == 1;
            }
        }

        public async Task<IEnumerable<Ingredient>> FindAllByName(int groupId, string name)
        {
            var query = $"select * from Ingredients where name like @name + '%' and groupId=@groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Ingredient>(query, new { name = name, groupId });
            }
        }

        public async Task<Ingredient> FindSingleByName(int groupId, string name)
        {
            var query = $"select * from Ingredients where name=@name and groupId=@groupId";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var items = await connection.QueryAsync<Ingredient>(query, new { groupId, name });
                return items.FirstOrDefault();
            }
        }

        public async Task<bool> Save(int groupId, Ingredient item)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                if (item.Id.HasValue)
                { 
                    var queryUpdate = $"update Ingredients set name=@name where id=@id and groupId=@groupId";

                    var updateCommand = new SqlCommand(queryUpdate, connection);
                    updateCommand.Parameters.Add(new SqlParameter("name", item.Name));
                    updateCommand.Parameters.Add(new SqlParameter("id", item.Id));
                    updateCommand.Parameters.Add(new SqlParameter("groupId", groupId));
                    return await updateCommand.ExecuteNonQueryAsync() == 1;
                }
                else
                {
                    var queryInsert = @"insert into Ingredients(name, groupId) values (@name, @groupId);
                                        select SCOPE_IDENTITY();";
                    var result = await connection.QueryAsync<int>(queryInsert, new { name = item.Name, groupId });
                    if (result.Any())
                    {
                        item.Id = result.FirstOrDefault();
                    }
                    return true;
                }
            }
        }

        public async Task<IEnumerable<Ingredient>> SearchByPart(int groupId, string part)
        {
            var query = $"select * from Ingredients where name like '%'+@part + '%' and @groupid=groupid";
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Ingredient>(query, new { groupId,  part });
            }
        }
    }
}
