using BudgetApp.Enums;
using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface IBudgetInviteRepository<T>
        where T : BudgetInviteModel
    {
        Task<IEnumerable<T>> GetByBudgetId(int budgetId);
        Task<IEnumerable<T>> GetPendingByInvitedUserId(int userId);
        Task<int> GetPendingCountByInvitedUserId(int userId);
        Task<T?> GetById(int id);
        Task<int> Create(T invite);
        Task<int> UpdateStatus(int id, InviteStatus status);
        Task<int> Reinvite(int id, int invitedByUserId);
    }
}
