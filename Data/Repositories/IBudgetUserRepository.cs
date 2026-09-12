using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface IBudgetUserRepository<T>
        where T : BudgetUserModel
    {
        Task<IEnumerable<T>> GetByBudgetId(int budgetId);
        Task<IEnumerable<T>> GetByUserId(int userId);
        Task<int> Create(T budgetUser);
        Task<int> Delete(int id);
    }
}
