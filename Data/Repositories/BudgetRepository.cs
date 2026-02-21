using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class BudgetRepository<T> : BaseRepository, IBudgetRepository<T> where T : BudgetModel
    {

        public BudgetRepository(DapperContext context) : base(context) { }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[Name]
                  ,[Description]
                  ,[CampId]
              FROM [dbo].[Budget]
            ";
            return await conn.QueryAsync<T>(sql);
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
                  ,[CampId]
              FROM [dbo].[Budget]
              WHERE Id = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<IEnumerable<T>> GetByCampId(int campId)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[Name]
                  ,[Description]
                  ,[CampId]
              FROM [dbo].[Budget]
              WHERE [CampId] = @CampId
            ";
            return await conn.QueryAsync<T>(sql, new { CampId = campId });
        }

        public async Task<int> Create(T budget)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            INSERT INTO [dbo].[Budget]
                       ([Name]
                       ,[Description]
                       ,[CampId])
                 VALUES
                       (@Name
                       ,@Description
                       ,@CampId);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, budget);
        }

        public async Task<int> Update(T budget)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            UPDATE [dbo].[Budget]
               SET [Name] = @Name
                  ,[Description] = @Description
                  ,[CampId] = @CampId
                WHERE Id = @Id
            ";
            return await conn.ExecuteAsync(sql, budget);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[Budget] WHERE Id = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
