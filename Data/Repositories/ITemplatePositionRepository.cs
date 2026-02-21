using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ITemplatePositionRepository<T> where T : TemplatePositionModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<IEnumerable<T>> GetByTemplateBudgetId(int templateBudgetId);
        Task<int> Create(T templatePosition);
        Task<int> Update(T templatePosition);
        Task<int> Delete(int id);
    }
}
