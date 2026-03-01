using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ITransactionRepository<T> where T : TransactionModel
    {
        Task<IEnumerable<T>> GetByCampId(int campId);
        Task<IEnumerable<T>> GetByUserId(int userId, int campId);
        Task<IEnumerable<UserExpenseSummaryViewModel>> GetUserSummariesByCampId(int campId);
        Task<T?> GetById(int id);
        Task<int> Create(T transaction);
        Task<int> Update(T transaction);
        Task<int> Delete(int id);
    }
}
