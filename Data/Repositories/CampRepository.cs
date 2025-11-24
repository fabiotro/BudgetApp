using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class CampRepository<T> : BaseRepository, ICampRepository<T> where T : CampModel
    {

        public CampRepository(DapperContext context) : base(context) { }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[StartDate]
                  ,[EndDate]
                  ,[MainLeader]
                  ,[ParticipantsCount_fc]
                  ,[js_PersonsCount_fc]
                  ,[LeadersTeamCount_fc]
                  ,[ParticipantsCount_rl]
                  ,[js_PersonsCount_rl]
                  ,[LeadersTeamCount_rl]
              FROM [dbo].[Camp]
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
                  ,[StartDate]
                  ,[EndDate]
                  ,[MainLeader]
                  ,[ParticipantsCount_fc]
                  ,[js_PersonsCount_fc]
                  ,[LeadersTeamCount_fc]
                  ,[ParticipantsCount_rl]
                  ,[js_PersonsCount_rl]
                  ,[LeadersTeamCount_rl]
                  ,[CreateDate]
                  ,[ChangeDate]
              FROM [dbo].[Camp]
              WHERE Id = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<int> Create(T camp)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            INSERT INTO [dbo].[Camp]
                        ([StartDate]
                        ,[EndDate]
                        ,[MainLeader]
                        ,[ParticipantsCount_fc]
                        ,[js_PersonsCount_fc]
                        ,[LeadersTeamCount_fc]
                        ,[ParticipantsCount_rl]
                        ,[js_PersonsCount_rl]
                        ,[LeadersTeamCount_rl])
                    VALUES
                        (@StartDate
                        ,@EndDate
                        ,@MainLeader
                        ,@ParticipantsCount_fc
                        ,@js_PersonsCount_fc
                        ,@LeadersTeamCount_fc
                        ,@ParticipantsCount_rl
                        ,@js_PersonsCount_rl
                        ,@LeadersTeamCount_rl);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, camp);
        }

        public async Task<int> Update(T camp)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            UPDATE [dbo].[Camp]
               SET [StartDate] = @StartDate
                  ,[EndDate] = @EndDate
                  ,[MainLeader] = @MainLeader
                  ,[ParticipantsCount_fc] = @ParticipantsCount_fc
                  ,[js_PersonsCount_fc] = @js_PersonsCount_fc
                  ,[LeadersTeamCount_fc] = @LeadersTeamCount_fc
                  ,[ParticipantsCount_rl] = @ParticipantsCount_rl
                  ,[js_PersonsCount_rl] = @js_PersonsCount_rl
                  ,[LeadersTeamCount_rl] = @LeadersTeamCount_rl
             WHERE Id = @Id
            ";
            return await conn.ExecuteAsync(sql, camp);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[Camp] WHERE Id = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
