using Dapper;
using Microsoft.Extensions.Configuration;
using System;
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

        public async Task<string> GetByName(string name)
        {
            using (var connection = new SqlConnection(this.connectionString))
            {
                var query = @"select GroupGuid from Groups where name = @name";
                var result = await connection.QueryAsync<Guid>(query, new { name });
                var guid = result.FirstOrDefault();
                if (!guid.Equals(Guid.Empty))
                {
                    return guid.ToString();
                }
                return null;
            }
        }
    }
}
