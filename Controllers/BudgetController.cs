using BudgetApp.Data.Repositories;
using BudgetApp.Enums;
using BudgetApp.Extensions;
using BudgetApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.Controllers
{
    public class BudgetController : Controller
    {
        private readonly ICampRepository<CampModel> _campRepo;

        public BudgetController(ICampRepository<CampModel> campRepo)
        {
            _campRepo = campRepo;
        }

        [HttpGet]
        public async Task<IActionResult> NewBudget(int? campId)
        {
            var newBudget = new NewBudgetViewModel();

            if (campId.HasValue)
            {
                var camp = await _campRepo.GetById(campId.Value);
                if (camp != null)
                    newBudget.Camp = MapModelToViewModel(camp);
            }
            return View(newBudget);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertCamp(NewBudgetViewModel newBudget)
        {
            if (!ModelState.IsValid)
                return View(nameof(NewBudget), newBudget);

            var toast = new ToastMessageViewModel();

            try
            {
                var campModel = MapViewModelToModel(newBudget.Camp);

                if (newBudget.Camp.Id == 0)
                {
                    // CREATE
                    int newId = await _campRepo.Create(campModel);
                    newBudget.Camp.Id = newId;
                    toast = new ToastMessageViewModel
                    {
                        Title = "Erfolg",
                        Message = "Lagerdaten hinzugefügt.",
                        Type = ToastType.Success
                    };
                }
                else
                {
                    // UPDATE
                    await _campRepo.Update(campModel);
                    toast = new ToastMessageViewModel
                    {
                        Title = "Erfolg",
                        Message = "Lagerdaten aktualisiert.",
                        Type = ToastType.Success
                    };
                }
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(NewBudget), new { campId = newBudget.Camp.Id });
            }
            catch (Exception ex)
            {
                // TODO: Log exception with NLog
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
                TempData.Put("ToastMsg", toast);
                return View("NewBudget", newBudget);
            }
        }

        #region Helpers
        private CampModel MapViewModelToModel(CampViewModel campViewModel)
        {
            return new CampModel
            {
                Id = campViewModel.Id,
                StartDate = campViewModel.StartDate,
                EndDate = campViewModel.EndDate,
                MainLeader = campViewModel.MainLeader,
                ParticipantsCount_fc = campViewModel.ParticipantsCount_fc ?? 0,
                js_PersonsCount_fc = campViewModel.js_PersonsCount_fc ?? 0,
                LeadersTeamCount_fc = campViewModel.LeadersTeamCount_fc ?? 0,
                ParticipantsCount_rl = campViewModel.ParticipantsCount_rl,
                js_PersonsCount_rl = campViewModel.js_PersonsCount_rl,
                LeadersTeamCount_rl = campViewModel.LeadersTeamCount_rl
            };
        }

        private CampViewModel MapModelToViewModel(CampModel campModel)
        {
            return new CampViewModel
            {
                Id = campModel.Id,
                StartDate = campModel.StartDate,
                EndDate = campModel.EndDate,
                MainLeader = campModel.MainLeader,
                ParticipantsCount_fc = campModel.ParticipantsCount_fc,
                js_PersonsCount_fc = campModel.js_PersonsCount_fc,
                LeadersTeamCount_fc = campModel.LeadersTeamCount_fc,
                ParticipantsCount_rl = campModel.ParticipantsCount_rl,
                js_PersonsCount_rl = campModel.js_PersonsCount_rl,
                LeadersTeamCount_rl = campModel.LeadersTeamCount_rl
            };
        }
        #endregion
    }
}
