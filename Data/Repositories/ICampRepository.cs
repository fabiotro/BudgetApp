using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ICampRepository<T> where T : CampModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<int> Create(T camp);
        Task<int> Update(T camp);
        Task<int> Delete(int id);
    }
}
