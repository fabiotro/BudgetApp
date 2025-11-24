using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class CategoryRepository<T> : ICategoryRepository<T> where T : CategoryModel
    {
        private readonly DapperContext _context;

        public CategoryRepository(DapperContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[Name]
                  ,[Description]
                  ,[SortIndex]
              FROM [dbo].[Category]
            ";
            return await conn.QueryAsync<T>(sql);
        }

        public async Task<T?> GetById(int id)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[Name]
                  ,[Description]
                  ,[SortIndex]
              FROM [dbo].[Category]
              WHERE Id = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> Create(T category)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            INSERT INTO [dbo].[Category]
                       ([Name]
                       ,[Description]
                       ,[SortIndex])
                 VALUES
                       (@Name
                       ,@Description
                       ,@SortIndex);
                 SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, category);
        }

        public async Task<int> Update(T category)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            UPDATE [dbo].[Category]
               SET [Name] = @Name
                  ,[Description] = @Description
                  ,[SortIndex] = @SortIndex
                WHERE Id = @Id
            ";
            return await conn.ExecuteAsync(sql, category);
        }

        public async Task<int> Delete(int id)
        {
            if (id <= 0)
                throw new ArgumentOutOfRangeException(nameof(id), "Die ID muss grösser als Null sein.");
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[Category] WHERE Id = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
