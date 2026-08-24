using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ICategoryRepository<T>
        where T : CategoryModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<int> Create(T category);
        Task<int> Update(T category);
        Task<int> Delete(int id);
    }
}
