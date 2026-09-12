using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class TemplateBudgetRepository<T> : BaseRepository, ITemplateBudgetRepository<T>
        where T : TemplateBudgetModel
    {
        public TemplateBudgetRepository(DapperContext context)
            : base(context) { }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                  ,[Name]
                  ,[Description]
                  ,[CreatedByUserId]
              FROM [dbo].[TemplateBudget]
            ";
            return await conn.QueryAsync<T>(sql);
        }

        public async Task<IEnumerable<T>> GetAllForUser(int userId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                  ,[Name]
                  ,[Description]
                  ,[CreatedByUserId]
              FROM [dbo].[TemplateBudget]
              WHERE [CreatedByUserId] = @UserId
            ";
            return await conn.QueryAsync<T>(sql, new { UserId = userId });
        }

        public async Task<T?> GetById(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                  ,[Name]
                  ,[Description]
                  ,[CreatedByUserId]
              FROM [dbo].[TemplateBudget]
              WHERE [Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> Create(T templateBudget)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            INSERT INTO [dbo].[TemplateBudget]
                       ([Name]
                       ,[Description]
                       ,[CreatedByUserId])
                 VALUES
                       (@Name
                       ,@Description
                       ,@CreatedByUserId);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, templateBudget);
        }

        public async Task<int> Update(T templateBudget)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            UPDATE [dbo].[TemplateBudget]
               SET [Name] = @Name
                  ,[Description] = @Description
                WHERE [Id] = @Id
            ";
            return await conn.ExecuteAsync(sql, templateBudget);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[TemplateBudget] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
