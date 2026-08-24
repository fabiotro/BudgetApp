using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface IPositionTypeRepository<T>
        where T : PositionTypeModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<int> Create(T positionType);
        Task<int> Update(T positionType);
        Task<int> Delete(int id);
    }
}
