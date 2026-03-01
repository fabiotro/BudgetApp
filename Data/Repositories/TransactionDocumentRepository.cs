using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class TransactionDocumentRepository<T> : BaseRepository, ITransactionDocumentRepository<T>
        where T : TransactionDocumentModel
    {
        public TransactionDocumentRepository(DapperContext context) : base(context) { }

        public async Task<IEnumerable<T>> GetByTransactionId(int transactionId)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[TransactionId]
                  ,[FileName]
                  ,[ContentType]
                  ,[FileSize]
              FROM [dbo].[TransactionDocument]
              WHERE [TransactionId] = @TransactionId
              ORDER BY [CreateDate]
            ";
            // FileData is deliberately excluded from this query
            return await conn.QueryAsync<T>(sql, new { TransactionId = transactionId });
        }

        public async Task<T?> GetById(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[TransactionId]
                  ,[FileName]
                  ,[ContentType]
                  ,[FileSize]
                  ,[FileData]
              FROM [dbo].[TransactionDocument]
              WHERE [Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> Create(T document)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            INSERT INTO [dbo].[TransactionDocument]
                       ([TransactionId]
                       ,[FileName]
                       ,[ContentType]
                       ,[FileSize]
                       ,[FileData])
                 VALUES
                       (@TransactionId
                       ,@FileName
                       ,@ContentType
                       ,@FileSize
                       ,@FileData);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, document);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[TransactionDocument] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
