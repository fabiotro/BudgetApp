using BudgetApp.Models;

namespace BudgetApp.Extensions
{
    public static class BudgetUserExtensions
    {
        public static bool IsMainLeaderFor(
            this IEnumerable<BudgetUserModel> budgetUsers,
            int userId
        )
        {
            return budgetUsers.Any(bu => bu.UserId == userId && bu.IsMainLeader);
        }
    }
}
