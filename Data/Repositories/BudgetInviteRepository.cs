using BudgetApp.Enums;
using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class BudgetInviteRepository<T> : BaseRepository, IBudgetInviteRepository<T>
        where T : BudgetInviteModel
    {
        public BudgetInviteRepository(DapperContext context)
            : base(context) { }

        private const string SelectColumns =
            @"
                   ci.[Id]
                  ,ci.[BudgetId]
                  ,ci.[InvitedByUserId]
                  ,ci.[InvitedUserId]
                  ,ci.[Status]
                  ,ub.[DisplayName] AS InvitedByDisplayName
                  ,ui.[DisplayName] AS InvitedUserDisplayName
                  ,ui.[Email]       AS InvitedUserEmail
                  ,b.[Name]         AS BudgetName
            ";

        private const string Joins =
            @"
              FROM [dbo].[BudgetInvite] ci
              INNER JOIN [dbo].[User] ub ON ci.[InvitedByUserId] = ub.[Id]
              INNER JOIN [dbo].[User] ui ON ci.[InvitedUserId]   = ui.[Id]
              INNER JOIN [dbo].[Budget] b ON ci.[BudgetId]       = b.[Id]
            ";

        public async Task<IEnumerable<T>> GetByBudgetId(int budgetId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                $@"
            SELECT {SelectColumns}
            {Joins}
              WHERE ci.[BudgetId] = @BudgetId
              ORDER BY ci.[CreateDate] DESC
            ";
            return await conn.QueryAsync<T>(sql, new { BudgetId = budgetId });
        }

        public async Task<IEnumerable<T>> GetPendingByInvitedUserId(int userId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                $@"
            SELECT {SelectColumns}
            {Joins}
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
              FROM [dbo].[BudgetInvite]
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
                $@"
            SELECT {SelectColumns}
            {Joins}
              WHERE ci.[Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> Create(T invite)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            INSERT INTO [dbo].[BudgetInvite]
                       ([BudgetId]
                       ,[InvitedByUserId]
                       ,[InvitedUserId]
                       ,[Status])
                 VALUES
                       (@BudgetId
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
            var sql = "UPDATE [dbo].[BudgetInvite] SET [Status] = @Status WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id, Status = (int)status });
        }

        public async Task<int> Reinvite(int id, int invitedByUserId)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql =
                @"
            UPDATE [dbo].[BudgetInvite]
               SET [Status] = 0
                  ,[InvitedByUserId] = @InvitedByUserId
             WHERE [Id] = @Id
            ";
            return await conn.ExecuteAsync(sql, new { Id = id, InvitedByUserId = invitedByUserId });
        }
    }
}
