using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class CampUserRepository<T> : BaseRepository, ICampUserRepository<T>
        where T : CampUserModel
    {
        public CampUserRepository(DapperContext context)
            : base(context) { }

        public async Task<IEnumerable<T>> GetByCampId(int campId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT cu.[Id]
                  ,cu.[CampId]
                  ,cu.[UserId]
                  ,cu.[IsMainLeader]
                  ,u.[DisplayName]
                  ,u.[Email]
              FROM [dbo].[CampUser] cu
              INNER JOIN [dbo].[User] u ON cu.[UserId] = u.[Id]
              WHERE cu.[CampId] = @CampId
              ORDER BY cu.[IsMainLeader] DESC, u.[DisplayName]
            ";
            return await conn.QueryAsync<T>(sql, new { CampId = campId });
        }

        public async Task<IEnumerable<T>> GetByUserId(int userId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT cu.[Id]
                  ,cu.[CampId]
                  ,cu.[UserId]
                  ,cu.[IsMainLeader]
                  ,u.[DisplayName]
                  ,u.[Email]
              FROM [dbo].[CampUser] cu
              INNER JOIN [dbo].[User] u ON cu.[UserId] = u.[Id]
              WHERE cu.[UserId] = @UserId
            ";
            return await conn.QueryAsync<T>(sql, new { UserId = userId });
        }

        public async Task<int> Create(T campUser)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            INSERT INTO [dbo].[CampUser]
                       ([CampId]
                       ,[UserId]
                       ,[IsMainLeader])
                 VALUES
                       (@CampId
                       ,@UserId
                       ,@IsMainLeader);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, campUser);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[CampUser] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
