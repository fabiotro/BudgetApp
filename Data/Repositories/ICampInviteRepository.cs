using BudgetApp.Enums;
using BudgetApp.Models;

namespace BudgetApp.Data.Repositories
{
    public interface ICampInviteRepository<T>
        where T : CampInviteModel
    {
        Task<IEnumerable<T>> GetByCampId(int campId);
        Task<IEnumerable<T>> GetPendingByInvitedUserId(int userId);
        Task<int> GetPendingCountByInvitedUserId(int userId);
        Task<T?> GetById(int id);
        Task<int> Create(T invite);
        Task<int> UpdateStatus(int id, InviteStatus status);
        Task<int> Reinvite(int id, int invitedByUserId);
    }
}
