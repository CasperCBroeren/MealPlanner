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
                if (item.GroupId.HasValue)
                {
                    var queryInsert = @"insert into Groups(GroupGuid, name) values (@guid, @name);
                                        select SCOPE_IDENTITY();";
                    var result = await connection.QueryAsync<int>(queryInsert, new { name = item.Name, guid = item.GroupId });
                    if (result.Any())
                    {
                        item.GroupId = result.First();
                    }
                    return true;

                }
                else
                {
                    var queryUpdate = $"update groups set name=@name where id=@id and groupId=@id";

                    var updateCommand = new SqlCommand(queryUpdate, connection);
                    updateCommand.Parameters.Add(new SqlParameter("name", item.Name));
                    updateCommand.Parameters.Add(new SqlParameter("id", item.GroupId)); 
                    return await updateCommand.ExecuteNonQueryAsync() == 1;
                }
            }
        } 

        public async Task<Group> GetByName(string name)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var query = @"select GroupId, GroupGuid, Name, Created from Groups where name = @name";
                var result = await connection.QueryAsync<Group>(query, new { name });
                return result.FirstOrDefault();
            }
        }
    }
}
