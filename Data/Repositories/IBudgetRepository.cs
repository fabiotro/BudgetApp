using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface IBudgetRepository<T>
        where T : BudgetModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<IEnumerable<T>> GetAllForUser(int userId);
        Task<T?> GetById(int id);
        Task<IEnumerable<T>> GetByCampId(int campId);
        Task<int> Create(T budget);
        Task<int> Update(T budget);
        Task<int> Delete(int id);
    }
}
