using Dapper;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MealPlanner.Data.Repositories.Dapper
{
    public class GroupRepository : IGroupRepository
    {
        public string connectionString { get; }

        public GroupRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<string> AddNew(string name)
        {
            var guid = Guid.NewGuid();
            using (var connection = new SqlConnection(this.connectionString))
            {
                var queryInsert = @"insert into Groups(GroupGuid, name) values (@guid, @name);
                                        select SCOPE_IDENTITY();";
                await connection.QueryAsync<int>(queryInsert, new { name = name, guid = guid }); 
            }
            return guid.ToString();
        }

        public async Task<bool> ExistsByName(string groupName)
        {
            var guid = Guid.NewGuid();
            using (var connection = new SqlConnection(this.connectionString))
            {
                var query = @"select 1 from Groups where Name = @name";
                var result = await connection.QueryAsync<int>(query, new { name = groupName });
                return result.FirstOrDefault() == 1;
            } 
        }

        public async Task<string> GetName(string groupGuid)
        { 
            using (var connection = new SqlConnection(this.connectionString))
            {
                var query = @"select name from Groups where GroupGuid = @guid";
                var result = await connection.QueryAsync<string>(query, new { guid = groupGuid });
                return result.FirstOrDefault();
            }
        }

        public async Task<int> GetId(string groupGuid)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var query = @"select groupId from Groups where GroupGuid = @guid";
                var result = await connection.QueryAsync<int>(query, new { guid = groupGuid });
                return result.FirstOrDefault();
            }
        }
    }
}
