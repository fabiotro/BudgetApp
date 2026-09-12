using BudgetApp.Models;
using Dapper;

namespace BudgetApp.Data.Repositories
{
    public class TemplatePositionRepository<T> : BaseRepository, ITemplatePositionRepository<T>
        where T : TemplatePositionModel
    {
        public TemplatePositionRepository(DapperContext context)
            : base(context) { }

        public async Task<IEnumerable<T>> GetAll()
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                  ,[TemplateBudgetId]
                  ,[PositionTypeId]
                  ,[CategoryId]
                  ,[SubCategoryId]
                  ,[Name]
                  ,[FixedAmount]
                  ,[Quantity]
                  ,[QuantityVar]
                  ,[UnitAmount]
                  ,[SortIndex]
              FROM [dbo].[TemplatePosition]
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
                  ,[TemplateBudgetId]
                  ,[PositionTypeId]
                  ,[CategoryId]
                  ,[SubCategoryId]
                  ,[Name]
                  ,[FixedAmount]
                  ,[Quantity]
                  ,[QuantityVar]
                  ,[UnitAmount]
                  ,[SortIndex]
                FROM [dbo].[TemplatePosition]
                WHERE [Id] = @Id
            ";
            return await conn.QuerySingleOrDefaultAsync<T>(sql, new { Id = id });
        }

        public async Task<IEnumerable<T>> GetByTemplateBudgetId(int templateBudgetId)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            SELECT [Id]
                  ,[TemplateBudgetId]
                  ,[PositionTypeId]
                  ,[CategoryId]
                  ,[SubCategoryId]
                  ,[Name]
                  ,[FixedAmount]
                  ,[Quantity]
                  ,[QuantityVar]
                  ,[UnitAmount]
                  ,[SortIndex]
              FROM [dbo].[TemplatePosition]
              WHERE [TemplateBudgetId] = @TemplateBudgetId
            ";
            return await conn.QueryAsync<T>(sql, new { TemplateBudgetId = templateBudgetId });
        }

        public async Task<int> Create(T templatePosition)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            INSERT INTO [dbo].[TemplatePosition]
                       ([TemplateBudgetId]
                       ,[PositionTypeId]
                       ,[CategoryId]
                       ,[SubCategoryId]
                       ,[Name]
                       ,[FixedAmount]
                       ,[Quantity]
                       ,[QuantityVar]
                       ,[UnitAmount]
                       ,[SortIndex])
                 VALUES
                       (@TemplateBudgetId
                       ,@PositionTypeId
                       ,@CategoryId
                       ,@SubCategoryId
                       ,@Name
                       ,@FixedAmount
                       ,@Quantity
                       ,@QuantityVar
                       ,@UnitAmount
                       ,@SortIndex);
                    SELECT CAST(SCOPE_IDENTITY() as int);
            ";
            return await conn.ExecuteScalarAsync<int>(sql, templatePosition);
        }

        public async Task<int> Update(T templatePosition)
        {
            using var conn = _context.CreateConnection();
            var sql =
                @"
            UPDATE [dbo].[TemplatePosition]
                SET [TemplateBudgetId] = @TemplateBudgetId
                    ,[PositionTypeId] = @PositionTypeId
                    ,[CategoryId] = @CategoryId
                    ,[SubCategoryId] = @SubCategoryId
                    ,[Name] = @Name
                    ,[FixedAmount] = @FixedAmount
                    ,[Quantity] = @Quantity
                    ,[QuantityVar] = @QuantityVar
                    ,[UnitAmount] = @UnitAmount
                    ,[SortIndex] = @SortIndex
                WHERE [Id] = @Id
            ";
            return await conn.ExecuteAsync(sql, templatePosition);
        }

        public async Task<int> Delete(int id)
        {
            ValidateId(id);
            using var conn = _context.CreateConnection();
            var sql = "DELETE FROM [dbo].[TemplatePosition] WHERE [Id] = @Id";
            return await conn.ExecuteAsync(sql, new { Id = id });
        }
    }
}
