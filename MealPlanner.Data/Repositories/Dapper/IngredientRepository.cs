using MealPlanner.Data.Models;
using System;
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
        public async Task<IEnumerable<Ingredient>> All()
        {
            var query = $"select * from Ingredients";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Ingredient>(query);
            }
        }

        public async Task<bool> Delete(Ingredient item)
        {
            var query = $"delete from Ingredients where id=@id";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.ExecuteAsync(query,new { id = item.Id }) == 1;
            }
        }

        public async Task<IEnumerable<Ingredient>> FindAllByName(string name)
        {
            var query = $"select * from Ingredients where name like @name + '%'";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Ingredient>(query, new { name = name });
            }
        }

        public async Task<Ingredient> FindSingleByName(string name)
        {
            var query = $"select * from Ingredients where name=@name";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                var items = await connection.QueryAsync<Ingredient>(query, new { name = name });
                return items.FirstOrDefault();
            }
        }

        public async Task<bool> Save(Ingredient item)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                if (item.Id.HasValue)
                { 
                    var queryUpdate = $"update Ingredients set name=@name where id=@id";

                    var updateCommand = new SqlCommand(queryUpdate, connection);
                    updateCommand.Parameters.Add(new SqlParameter("name", item.Name));
                    updateCommand.Parameters.Add(new SqlParameter("id", item.Id));
                    return await updateCommand.ExecuteNonQueryAsync() == 1;
                }
                else
                {
                    var queryInsert = @"insert into Ingredients(name) values (@name);
                                        select SCOPE_IDENTITY();";
                    var result = await connection.QueryAsync<int>(queryInsert, new { name = item.Name });
                    if (result.Any())
                    {
                        item.Id = result.FirstOrDefault();
                    }
                    return true;
                }
            }
        }

        public async Task<IEnumerable<Ingredient>> SearchByPart(string part)
        {
            var query = $"select * from Ingredients where name like '%'+@part + '%'";
            using (var connection = new SqlConnection(this.connectionString))
            {
                connection.Open();
                return await connection.QueryAsync<Ingredient>(query, new { part = part });
            }
        }
    }
}
