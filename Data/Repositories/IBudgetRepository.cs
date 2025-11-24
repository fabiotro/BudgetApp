using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface IBudgetRepository<T> where T : BudgetModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<int> Create(T budget);
        Task<int> Update(T budget);
        Task<int> Delete(int id);
    }
}
