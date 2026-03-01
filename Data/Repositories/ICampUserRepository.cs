using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ICampUserRepository<T> where T : CampUserModel
    {
        Task<IEnumerable<T>> GetByCampId(int campId);
        Task<IEnumerable<T>> GetByUserId(int userId);
        Task<int> Create(T campUser);
        Task<int> Delete(int id);
    }
}
