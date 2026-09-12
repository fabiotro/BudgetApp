using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class TransactionRepository<T> : BaseRepository, ITransactionRepository<T>
        where T : TransactionModel
    {
        public TransactionRepository(DapperContext context)
            : base(context) { }

        public async Task<IEnumerable<T>> GetByCampId(int campId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                  ,[CampId]
                  ,[PerformedByUserId]
                  ,[PositionId]
                  ,[Name]
                  ,[Description]
                  ,[Amount]
                  ,[PaymentSource]
                  ,[PaymentMethod]
              FROM [dbo].[Transaction]
              WHERE [CampId] = @CampId
              ORDER BY [CreateDate] DESC
            ";
            return await conn.QueryAsync<T>(sql, new { CampId = campId });
        }

        public async Task<IEnumerable<T>> GetByUserId(int userId, int campId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                  ,[CampId]
                  ,[PerformedByUserId]
                  ,[PositionId]
                  ,[Name]
                  ,[Description]
                  ,[Amount]
                  ,[PaymentSource]
                  ,[PaymentMethod]
              FROM [dbo].[Transaction]
              WHERE [PerformedByUserId] = @UserId AND [CampId] = @CampId
              ORDER BY [CreateDate] DESC
            ";
            return await conn.QueryAsync<T>(sql, new { UserId = userId, CampId = campId });
        }

        public async Task<IEnumerable<UserExpenseSummaryViewModel>> GetUserSummariesByCampId(
            int campId
        )
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT u.[Id]              AS UserId,
                   u.[DisplayName],
                   u.[FirstName],
                   u.[LastName],
                   u.[IBAN],
                   ISNULL(SUM(t.[Amount]), 0) AS TotalExpenses,
                   ISNULL(SUM(CASE WHEN t.[PaymentSource] = 1 THEN t.[Amount] ELSE 0 END), 0) AS PersonalMoneyOwed
              FROM [dbo].[CampUser] cu
              INNER JOIN [dbo].[User] u ON cu.[UserId] = u.[Id]
              LEFT JOIN [dbo].[Transaction] t ON t.[PerformedByUserId] = u.[Id] AND t.[CampId] = cu.[CampId]
              WHERE cu.[CampId] = @CampId
              GROUP BY u.[Id], u.[DisplayName], u.[FirstName], u.[LastName], u.[IBAN]
              ORDER BY u.[DisplayName]
            ";
            return await conn.QueryAsync<UserExpenseSummaryViewModel>(sql, new { CampId = campId });
        }

        public async Task<T?> GetById(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                  ,[CampId]
                  ,[PerformedByUserId]
                  ,[PositionId]
                  ,[Name]
                  ,[Description]
                  ,[Amount]
                  ,[PaymentSource]
                  ,[PaymentMethod]
              FROM [dbo].[Transaction]
              WHERE [Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> Create(T transaction)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            INSERT INTO [dbo].[Transaction]
                       ([CampId]
                       ,[PerformedByUserId]
                       ,[PositionId]
                       ,[Name]
                       ,[Description]
                       ,[Amount]
                       ,[PaymentSource]
                       ,[PaymentMethod])
                 VALUES
                       (@CampId
                       ,@PerformedByUserId
                       ,@PositionId
                       ,@Name
                       ,@Description
                       ,@Amount
                       ,@PaymentSource
                       ,@PaymentMethod);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, transaction);
        }

        public async Task<int> Update(T transaction)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            UPDATE [dbo].[Transaction]
               SET [PerformedByUserId] = @PerformedByUserId
                  ,[PositionId]        = @PositionId
                  ,[Name]              = @Name
                  ,[Description]       = @Description
                  ,[Amount]            = @Amount
                  ,[PaymentSource]     = @PaymentSource
                  ,[PaymentMethod]     = @PaymentMethod
             WHERE [Id] = @Id
            ";
            return await conn.ExecuteAsync(sql, transaction);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            // TransactionDocument rows cascade automatically via FK
            var sql = "DELETE FROM [dbo].[Transaction] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
