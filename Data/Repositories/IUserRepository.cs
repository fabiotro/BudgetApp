using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface IUserRepository<T> where T : UserModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<T?> GetByEmail(string email);
        Task<int> Create(T user);
        Task<int> Update(T user);
        Task<int> Delete(int id);
    }
}
