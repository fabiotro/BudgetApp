using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface IPositionRepository<T> where T : PositionModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<IEnumerable<T>> GetByBudgetId(int budgetId);
        Task<int> Create(T position);
        Task<int> Update(T position);
        Task<int> Delete(int id);
        Task<int> DeleteByBudgetId(int budgetId);
    }
}
