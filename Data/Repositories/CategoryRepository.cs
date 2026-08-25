using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class CategoryRepository<T> : BaseRepository, ICategoryRepository<T>
        where T : CategoryModel
    {
        public CategoryRepository(DapperContext context)
            : base(context) { }

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
            ValidateId(id);
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
                WHERE Id = @Id
            ";
            return await conn.ExecuteAsync(sql, category);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[Category] WHERE Id = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }

        public async Task UpdateSortOrder(IEnumerable<(int Id, int SortIndex)> items)
        {
            using var conn = _context.CreateConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            var sql = "UPDATE [dbo].[Category] SET [SortIndex] = @SortIndex WHERE [Id] = @Id";
            foreach (var item in items)
            {
                await conn.ExecuteAsync(sql, new { item.Id, item.SortIndex }, transaction);
            }
            transaction.Commit();
        }
    }
}
