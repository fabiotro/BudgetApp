using BudgetApp.Enums;
using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class CampInviteRepository<T> : BaseRepository, ICampInviteRepository<T>
        where T : CampInviteModel
    {
        public CampInviteRepository(DapperContext context)
            : base(context) { }

        public async Task<IEnumerable<T>> GetByCampId(int campId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT ci.[Id]
                  ,ci.[CampId]
                  ,ci.[InvitedByUserId]
                  ,ci.[InvitedUserId]
                  ,ci.[Status]
                  ,ub.[DisplayName] AS InvitedByDisplayName
                  ,ui.[DisplayName] AS InvitedUserDisplayName
                  ,ui.[Email]       AS InvitedUserEmail
                  ,c.[StartDate]    AS CampStartDate
                  ,c.[EndDate]      AS CampEndDate
                  ,c.[MainLeader]   AS CampMainLeader
              FROM [dbo].[CampInvite] ci
              INNER JOIN [dbo].[User] ub ON ci.[InvitedByUserId] = ub.[Id]
              INNER JOIN [dbo].[User] ui ON ci.[InvitedUserId]   = ui.[Id]
              INNER JOIN [dbo].[Camp] c  ON ci.[CampId]          = c.[Id]
              WHERE ci.[CampId] = @CampId
              ORDER BY ci.[CreateDate] DESC
            ";
            return await conn.QueryAsync<T>(sql, new { CampId = campId });
        }

        public async Task<IEnumerable<T>> GetPendingByInvitedUserId(int userId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT ci.[Id]
                  ,ci.[CampId]
                  ,ci.[InvitedByUserId]
                  ,ci.[InvitedUserId]
                  ,ci.[Status]
                  ,ub.[DisplayName] AS InvitedByDisplayName
                  ,ui.[DisplayName] AS InvitedUserDisplayName
                  ,ui.[Email]       AS InvitedUserEmail
                  ,c.[StartDate]    AS CampStartDate
                  ,c.[EndDate]      AS CampEndDate
                  ,c.[MainLeader]   AS CampMainLeader
              FROM [dbo].[CampInvite] ci
              INNER JOIN [dbo].[User] ub ON ci.[InvitedByUserId] = ub.[Id]
              INNER JOIN [dbo].[User] ui ON ci.[InvitedUserId]   = ui.[Id]
              INNER JOIN [dbo].[Camp] c  ON ci.[CampId]          = c.[Id]
              WHERE ci.[InvitedUserId] = @UserId
                AND ci.[Status] = 0
              ORDER BY ci.[CreateDate] DESC
            ";
            return await conn.QueryAsync<T>(sql, new { UserId = userId });
        }

        public async Task<int> GetPendingCountByInvitedUserId(int userId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT COUNT(*)
              FROM [dbo].[CampInvite]
              WHERE [InvitedUserId] = @UserId
                AND [Status] = 0
            ";
            return await conn.ExecuteScalarAsync<int>(sql, new { UserId = userId });
        }

        public async Task<T?> GetById(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT ci.[Id]
                  ,ci.[CampId]
                  ,ci.[InvitedByUserId]
                  ,ci.[InvitedUserId]
                  ,ci.[Status]
                  ,ub.[DisplayName] AS InvitedByDisplayName
                  ,ui.[DisplayName] AS InvitedUserDisplayName
                  ,ui.[Email]       AS InvitedUserEmail
                  ,c.[StartDate]    AS CampStartDate
                  ,c.[EndDate]      AS CampEndDate
                  ,c.[MainLeader]   AS CampMainLeader
              FROM [dbo].[CampInvite] ci
              INNER JOIN [dbo].[User] ub ON ci.[InvitedByUserId] = ub.[Id]
              INNER JOIN [dbo].[User] ui ON ci.[InvitedUserId]   = ui.[Id]
              INNER JOIN [dbo].[Camp] c  ON ci.[CampId]          = c.[Id]
              WHERE ci.[Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> Create(T invite)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            INSERT INTO [dbo].[CampInvite]
                       ([CampId]
                       ,[InvitedByUserId]
                       ,[InvitedUserId]
                       ,[Status])
                 VALUES
                       (@CampId
                       ,@InvitedByUserId
                       ,@InvitedUserId
                       ,@Status);
            SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, invite);
        }

        public async Task<int> UpdateStatus(int id, InviteStatus status)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "UPDATE [dbo].[CampInvite] SET [Status] = @Status WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id, Status = (int)status });
        }

        public async Task<int> Reinvite(int id, int invitedByUserId)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql =
                @"
            UPDATE [dbo].[CampInvite]
               SET [Status] = 0
                  ,[InvitedByUserId] = @InvitedByUserId
             WHERE [Id] = @Id
            ";
            return await conn.ExecuteAsync(sql, new { Id = id, InvitedByUserId = invitedByUserId });
        }
    }
}
