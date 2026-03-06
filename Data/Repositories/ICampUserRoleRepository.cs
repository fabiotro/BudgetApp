using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ICampUserRoleRepository<T> where T : CampUserRoleModel
    {
        Task<IEnumerable<T>> GetAll();
        Task<T?> GetById(int id);
    }
}
