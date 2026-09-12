using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class BudgetUserRepository<T> : BaseRepository, IBudgetUserRepository<T>
        where T : BudgetUserModel
    {
        public BudgetUserRepository(DapperContext context)
            : base(context) { }

        public async Task<IEnumerable<T>> GetByBudgetId(int budgetId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT bu.[Id]
                  ,bu.[BudgetId]
                  ,bu.[UserId]
                  ,bu.[IsMainLeader]
                  ,u.[DisplayName]
                  ,u.[Email]
              FROM [dbo].[BudgetUser] bu
              INNER JOIN [dbo].[User] u ON bu.[UserId] = u.[Id]
              WHERE bu.[BudgetId] = @BudgetId
              ORDER BY bu.[IsMainLeader] DESC, u.[DisplayName]
            ";
            return await conn.QueryAsync<T>(sql, new { BudgetId = budgetId });
        }

        public async Task<IEnumerable<T>> GetByUserId(int userId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT bu.[Id]
                  ,bu.[BudgetId]
                  ,bu.[UserId]
                  ,bu.[IsMainLeader]
                  ,u.[DisplayName]
                  ,u.[Email]
              FROM [dbo].[BudgetUser] bu
              INNER JOIN [dbo].[User] u ON bu.[UserId] = u.[Id]
              WHERE bu.[UserId] = @UserId
            ";
            return await conn.QueryAsync<T>(sql, new { UserId = userId });
        }

        public async Task<int> Create(T budgetUser)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            INSERT INTO [dbo].[BudgetUser]
                       ([BudgetId]
                       ,[UserId]
                       ,[IsMainLeader])
                 VALUES
                       (@BudgetId
                       ,@UserId
                       ,@IsMainLeader);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, budgetUser);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[BudgetUser] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
