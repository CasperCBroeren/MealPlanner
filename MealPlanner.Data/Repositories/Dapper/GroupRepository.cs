using Dapper;
using MealPlanner.Data.Models;
using Microsoft.Extensions.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories.Dapper
{
    public class GroupRepository : IGroupRepository
    {
        public string connectionString { get; }

        public GroupRepository(IConfiguration config)
        {
            this.connectionString = config["dbConnectionString"];
        }

        public async Task<bool> Save(Group item)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                if (!item.GroupId.HasValue)
                {
                    var queryInsert = @"insert into Groups(name, secret, refreshtoken) values ( @name, @secret, @refreshToken);
                                        select SCOPE_IDENTITY();";
                    var result = await connection.QueryAsync<int>(queryInsert, new { name = item.Name, secret = item.Secret, refreshToken =item.RefreshToken });
                    if (result.Any())
                    {
                        item.GroupId = result.First();
                    }
                    return true;

                }
                else
                {
                    var queryUpdate = $"update groups set name=@name, secret=@secret, refreshToken=@refreshToken where  groupId=@id";

                    var updateCommand = new SqlCommand(queryUpdate, connection);
                    updateCommand.Parameters.Add(new SqlParameter("name", item.Name));
                    updateCommand.Parameters.Add(new SqlParameter("secret", item.Secret));
                    updateCommand.Parameters.Add(new SqlParameter("refreshToken", item.RefreshToken));
                    updateCommand.Parameters.Add(new SqlParameter("id", item.GroupId)); 
                    return await updateCommand.ExecuteNonQueryAsync() == 1;
                }
            }
        } 

        public async Task<Group> GetByName(string name)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var query = @"select GroupId, Name, Secret, Created, RefreshToken from Groups where name = @name";
                var result = await connection.QueryAsync<Group>(query, new { name });
                return result.FirstOrDefault();
            }
        }

        public async Task<Group> GetById(int groupId)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                await connection.OpenAsync();
                var query = @"select GroupId, Name, Secret, Created, RefreshToken from Groups where GroupId = @groupId";
                var result = await connection.QueryAsync<Group>(query, new { groupId });
                return result.FirstOrDefault();
            }
        }
    }
}
