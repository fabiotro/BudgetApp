using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class PositionTypeRepository<T> : BaseRepository, IPositionTypeRepository<T>
        where T : PositionTypeModel
    {
        public PositionTypeRepository(DapperContext context)
            : base(context) { }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                  ,[Name]
                  ,[Description]
              FROM [dbo].[PositionType]
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
                FROM [dbo].[PositionType]
                WHERE [Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> Create(T positionType)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            INSERT INTO [dbo].[PositionType]
                       ([Name]
                       ,[Description])
                 VALUES
                       (@Name
                       ,@Description);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, positionType);
        }

        public async Task<int> Update(T positionType)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            UPDATE [dbo].[PositionType]
               SET [Name] = @Name
                  ,[Description] = @Description
                WHERE [Id] = @Id
            ";
            return await conn.ExecuteAsync(sql, positionType);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[PositionType] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
