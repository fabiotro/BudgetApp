using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ISubCategoryRepository<T>
        where T : SubCategoryModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
        Task<int> Create(T subCategory);
        Task<int> Update(T subCategory);
        Task<int> Delete(int id);
        Task UpdateSortOrder(IEnumerable<(int Id, int SortIndex)> items);
    }
}
