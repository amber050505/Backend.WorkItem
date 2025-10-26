using Backend.WorkItem.Repository.Utility;
using Backend.WorkItem.Repository.Utility.Interface;
using Backend.WorkItem.Repository.WorkItem.Interface;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace Backend.WorkItem.Repository.WorkItem
{
    public class WorkItemRepository : IWorkItemRepository
    {
        private readonly IConnectionString _connStr;
        public WorkItemRepository(IConnectionString config)
        {
            _connStr = config;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connStr.DBConnString);

        public async Task<IEnumerable<Model.WorkItem>> GetAllAsync()
        {
            using var db = CreateConnection();
            return await db.QueryAsync<Model.WorkItem>("dbo.sp_WorkItem_GetAll", commandType: CommandType.StoredProcedure);
        }

        public async Task<Model.WorkItem?> GetByIdAsync(int id)
        {
            using var db = CreateConnection();
            return await db.QueryFirstOrDefaultAsync<Model.WorkItem>("dbo.sp_WorkItem_GetById", new { Id = id }, commandType: CommandType.StoredProcedure);
        }

        public async Task<int> CreateAsync(Model.WorkItem item)
        {
            using var db = CreateConnection();
            var newId = await db.ExecuteScalarAsync<int>(
                "dbo.sp_WorkItem_Merge",
                new { item.Title, item.Description },
                commandType: CommandType.StoredProcedure);
            return newId;
        }

        public async Task<bool> UpdateAsync(Model.WorkItem item)
        {
            using var db = CreateConnection();
            var rows = await db.ExecuteScalarAsync<int>(
                "dbo.sp_WorkItem_Merge",
                new { item.Id, item.Title, item.Description },
                commandType: CommandType.StoredProcedure);
            return rows > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var db = CreateConnection();
            var rows = await db.ExecuteScalarAsync<int>("dbo.sp_WorkItem_Delete", new { Id = id }, commandType: CommandType.StoredProcedure);
            return rows > 0;
        }
    }
}
