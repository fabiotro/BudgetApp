using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class UserRepository<T> : BaseRepository, IUserRepository<T> where T : UserModel
    {
        public UserRepository(DapperContext context) : base(context) { }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[Email]
                  ,[DisplayName]
                  ,[PasswordHash]
              FROM [dbo].[User]
              ORDER BY [DisplayName]
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
                  ,[Email]
                  ,[DisplayName]
                  ,[PasswordHash]
              FROM [dbo].[User]
              WHERE [Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<T?> GetByEmail(string email)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[Email]
                  ,[DisplayName]
                  ,[PasswordHash]
              FROM [dbo].[User]
              WHERE [Email] = @Email
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Email = email });
        }

        public async Task<int> Create(T user)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            INSERT INTO [dbo].[User]
                       ([Email]
                       ,[DisplayName]
                       ,[PasswordHash])
                 VALUES
                       (@Email
                       ,@DisplayName
                       ,@PasswordHash);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, user);
        }

        public async Task<int> Update(T user)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            UPDATE [dbo].[User]
               SET [Email] = @Email
                  ,[DisplayName] = @DisplayName
                  ,[PasswordHash] = @PasswordHash
             WHERE [Id] = @Id
            ";
            return await conn.ExecuteAsync(sql, user);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[User] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
