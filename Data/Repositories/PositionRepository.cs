using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class PositionRepository<T> : BaseRepository, IPositionRepository<T> where T : PositionModel
    {

        public PositionRepository(DapperContext context) : base(context) { }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[BudgetId]
                  ,[PositionTypeId]
                  ,[CategoryId]
                  ,[SubCategoryId]
                  ,[Name]
                  ,[FixedAmount_fc]
                  ,[Quantity_fc]
                  ,[UnitAmount_fc]
                  ,[FixedAmount_rl]
                  ,[Quantity_rl]
                  ,[UnitAmount_rl]
                  ,[SortIndex]
              FROM [dbo].[Position]
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
                  ,[BudgetId]
                  ,[PositionTypeId]
                  ,[CategoryId]
                  ,[SubCategoryId]
                  ,[Name]
                  ,[FixedAmount_fc]
                  ,[Quantity_fc]
                  ,[UnitAmount_fc]
                  ,[FixedAmount_rl]
                  ,[Quantity_rl]
                  ,[UnitAmount_rl]
                  ,[SortIndex]
                FROM [dbo].[Position]
                WHERE [Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<IEnumerable<T>> GetByBudgetId(int budgetId)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            SELECT [Id]
                  ,[BudgetId]
                  ,[PositionTypeId]
                  ,[CategoryId]
                  ,[SubCategoryId]
                  ,[Name]
                  ,[FixedAmount_fc]
                  ,[Quantity_fc]
                  ,[UnitAmount_fc]
                  ,[FixedAmount_rl]
                  ,[Quantity_rl]
                  ,[UnitAmount_rl]
                  ,[SortIndex]
              FROM [dbo].[Position]
              WHERE [BudgetId] = @BudgetId
            ";
            return await conn.QueryAsync<T>(sql, new { BudgetId = budgetId });
        }

        public async Task<int> Create(T position)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            INSERT INTO [dbo].[Position]
                    ([BudgetId]
                    ,[PositionTypeId]
                    ,[CategoryId]
                    ,[SubCategoryId]
                    ,[Name]
                    ,[FixedAmount_fc]
                    ,[Quantity_fc]
                    ,[UnitAmount_fc]
                    ,[FixedAmount_rl]
                    ,[Quantity_rl]
                    ,[UnitAmount_rl]
                    ,[SortIndex])
                VALUES
                    (@BudgetId
                    ,@PositionTypeId
                    ,@CategoryId
                    ,@SubCategoryId
                    ,@Name
                    ,@FixedAmount_fc
                    ,@Quantity_fc
                    ,@UnitAmount_fc
                    ,@FixedAmount_rl
                    ,@Quantity_rl
                    ,@UnitAmount_rl
                    ,@SortIndex);
                SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, position);
        }

        public async Task<int> Update(T position)
        {
            using var conn = _context.CreateConnection();
            var sql =
            @"
            UPDATE [dbo].[Position]
                SET [BudgetId] = @BudgetId
                    ,[PositionTypeId] = @PositionTypeId
                    ,[CategoryId] = @CategoryId
                    ,[SubCategoryId] = @SubCategoryId
                    ,[Name] = @Name
                    ,[FixedAmount_fc] = @FixedAmount_fc
                    ,[Quantity_fc] = @Quantity_fc
                    ,[UnitAmount_fc] = @UnitAmount_fc
                    ,[FixedAmount_rl] = @FixedAmount_rl
                    ,[Quantity_rl] = @Quantity_rl
                    ,[UnitAmount_rl] = @UnitAmount_rl
                    ,[SortIndex] = @SortIndex
                WHERE [Id] = @Id
            ";
            return await conn.ExecuteAsync(sql, position);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[Position] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
