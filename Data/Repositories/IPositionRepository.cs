using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface IPositionRepository<T> where T : PositionModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<int> Create(T position);
        Task<int> Update(T osition);
        Task<int> Delete(int id);
    }
}
