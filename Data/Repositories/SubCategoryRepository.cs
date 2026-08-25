using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class SubCategoryRepository<T> : BaseRepository, ISubCategoryRepository<T>
        where T : SubCategoryModel
    {
        public SubCategoryRepository(DapperContext context)
            : base(context) { }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                    ,[CategoryId]
                    ,[Name]
                    ,[Description]
                    ,[SortIndex]
                FROM [dbo].[SubCategory]
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
                  ,[CategoryId]
                  ,[Name]
                  ,[Description]
                  ,[SortIndex]
              FROM [dbo].[SubCategory]
              WHERE [Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> Create(T subCategory)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            INSERT INTO [dbo].[SubCategory]
                       ([CategoryId]
                       ,[Name]
                       ,[Description]
                       ,[SortIndex])
                 VALUES
                       (@CategoryId
                       ,@Name
                       ,@Description
                       ,@SortIndex);
                 SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, subCategory);
        }

        public async Task<int> Update(T subCategory)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            UPDATE [dbo].[SubCategory]
                SET [CategoryId] = @CategoryId
                    ,[Name] = @Name
                    ,[Description] = @Description
                WHERE [Id] = @Id
            ";
            return await conn.ExecuteAsync(sql, subCategory);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[SubCategory] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }

        public async Task UpdateSortOrder(IEnumerable<(int Id, int SortIndex)> items)
        {
            using var conn = _context.CreateConnection();
            conn.Open();
            using var transaction = conn.BeginTransaction();
            var sql = "UPDATE [dbo].[SubCategory] SET [SortIndex] = @SortIndex WHERE [Id] = @Id";
            foreach (var item in items)
            {
                await conn.ExecuteAsync(sql, new { item.Id, item.SortIndex }, transaction);
            }
            transaction.Commit();
        }
    }
}
