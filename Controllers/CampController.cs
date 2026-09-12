using System.Security.Claims;
using BudgetApp.Data.Repositories;
using BudgetApp.Enums;
using BudgetApp.Extensions;
using BudgetApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.Controllers
{
    [Authorize]
    public class CampController : Controller
    {
        private readonly ILogger<CampController> _logger;
        private readonly ICampRepository<CampModel> _campRepo;
        private readonly IBudgetRepository<BudgetModel> _budgetRepo;
        private readonly ITransactionRepository<TransactionModel> _transactionRepo;
        private readonly ICampUserRepository<CampUserModel> _campUserRepo;
        private readonly ICampInviteRepository<CampInviteModel> _inviteRepo;

        public CampController(
            ILogger<CampController> logger,
            ICampRepository<CampModel> campRepo,
            IBudgetRepository<BudgetModel> budgetRepo,
            ITransactionRepository<TransactionModel> transactionRepo,
            ICampUserRepository<CampUserModel> campUserRepo,
            ICampInviteRepository<CampInviteModel> inviteRepo
        )
        {
            _logger = logger;
            _campRepo = campRepo;
            _budgetRepo = budgetRepo;
            _transactionRepo = transactionRepo;
            _campUserRepo = campUserRepo;
            _inviteRepo = inviteRepo;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            int userId = GetCurrentUserId();

            var camp = await _campRepo.GetById(id);
            if (camp == null)
                return NotFound();

            var campUsers = (await _campUserRepo.GetByCampId(id)).ToList();
            bool hasAccess =
                camp.CreatedByUserId == userId || campUsers.Any(cu => cu.UserId == userId);
            if (!hasAccess)
                return Forbid();

            var budgets = (await _budgetRepo.GetByCampId(id)).ToList();
            var userSummaries = (await _transactionRepo.GetUserSummariesByCampId(id)).ToList();
            var invites = (await _inviteRepo.GetByCampId(id)).ToList();
            bool isMainLeader = campUsers.Any(cu => cu.UserId == userId && cu.IsMainLeader);

            var vm = new CampDetailViewModel
            {
                Camp = camp,
                Budgets = budgets,
                UserSummaries = userSummaries,
                CampUsers = campUsers,
                Invites = invites,
                IsMainLeader = isMainLeader,
                InviteForm = new SendInviteViewModel { Email = string.Empty, CampId = id },
            };
            return View(vm);
        }
    }
}
