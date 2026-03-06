using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class CampUserRoleRepository<T> : BaseRepository, ICampUserRoleRepository<T> where T : CampUserRoleModel
    {
        public CampUserRoleRepository(DapperContext context) : base(context) { }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[Name]
              FROM [dbo].[CampUserRole]
              ORDER BY [Id]
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
              FROM [dbo].[CampUserRole]
              WHERE [Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }
    }
}
