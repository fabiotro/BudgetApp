using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ITransactionRepository<T>
        where T : TransactionModel
    {
        Task<IEnumerable<T>> GetByBudgetId(int budgetId);
        Task<IEnumerable<T>> GetByUserId(int userId, int budgetId);
        Task<IEnumerable<UserExpenseSummaryViewModel>> GetUserSummariesByBudgetId(int budgetId);
        Task<T?> GetById(int id);
        Task<int> Create(T transaction);
        Task<int> Update(T transaction);
        Task<int> Delete(int id);
    }
}
