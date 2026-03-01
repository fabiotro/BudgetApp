using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ITemplateBudgetRepository<T> where T : TemplateBudgetModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAllForUser(int userId);
        Task<T?> GetById(int id);
        Task<int> Create(T templateBudget);
        Task<int> Update(T templateBudget);
        Task<int> Delete(int id);
    }
}
