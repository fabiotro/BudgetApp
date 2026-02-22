using BudgetApp.Data.Repositories;
using BudgetApp.Enums;
using BudgetApp.Extensions;
using BudgetApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.Controllers
{
    public class BudgetController : Controller
    {
        private readonly ILogger<BudgetController> _logger;
        private readonly ICampRepository<CampModel> _campRepo;
        private readonly IBudgetRepository<BudgetModel> _budgetRepo;
        private readonly ITemplateBudgetRepository<TemplateBudgetModel> _templateBudgetRepo;
        private readonly ITemplatePositionRepository<TemplatePositionModel> _templatePositionRepo;
        private readonly IPositionRepository<PositionModel> _positionRepo;
        private readonly IPositionTypeRepository<PositionTypeModel> _positionTypeRepo;
        private readonly ICategoryRepository<CategoryModel> _categoryRepo;
        private readonly ISubCategoryRepository<SubCategoryModel> _subCategoryRepo;

        public BudgetController(
            ILogger<BudgetController> logger,
            ICampRepository<CampModel> campRepo,
            IBudgetRepository<BudgetModel> budgetRepo,
            ITemplateBudgetRepository<TemplateBudgetModel> templateBudgetRepo,
            ITemplatePositionRepository<TemplatePositionModel> templatePositionRepo,
            IPositionRepository<PositionModel> positionRepo,
            IPositionTypeRepository<PositionTypeModel> positionTypeRepo,
            ICategoryRepository<CategoryModel> categoryRepo,
            ISubCategoryRepository<SubCategoryModel> subCategoryRepo)
        {
            _logger = logger;
            _campRepo = campRepo;
            _budgetRepo = budgetRepo;
            _templateBudgetRepo = templateBudgetRepo;
            _templatePositionRepo = templatePositionRepo;
            _positionRepo = positionRepo;
            _positionTypeRepo = positionTypeRepo;
            _categoryRepo = categoryRepo;
            _subCategoryRepo = subCategoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var budgets = (await _budgetRepo.GetAll()).ToList();
            var camps = (await _campRepo.GetAll()).ToDictionary(c => c.Id);

            var vm = budgets
                .Select(b =>
                {
                    camps.TryGetValue(b.CampId, out var camp);
                    return new BudgetListViewModel { Budget = b, Camp = camp };
                })
                .ToList();

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> NewBudget(int? campId)
        {
            var vm = new NewBudgetViewModel
            {
                Camp = new CampViewModel
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(7)
                },
                AvailableTemplates = (await _templateBudgetRepo.GetAll()).ToList()
            };

            if (campId.HasValue)
            {
                var camp = await _campRepo.GetById(campId.Value);
                if (camp != null)
                {
                    vm.Camp = MapModelToViewModel(camp);
                    vm.ExistingBudgets = (await _budgetRepo.GetByCampId(campId.Value)).ToList();
                }
            }

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertCamp(NewBudgetViewModel newBudget)
        {
            if (!ModelState.IsValid)
            {
                newBudget.AvailableTemplates = (await _templateBudgetRepo.GetAll()).ToList();
                if (newBudget.Camp.Id > 0)
                    newBudget.ExistingBudgets = (await _budgetRepo.GetByCampId(newBudget.Camp.Id)).ToList();
                return View(nameof(NewBudget), newBudget);
            }

            var toast = new ToastMessageViewModel();

            try
            {
                var campModel = MapViewModelToModel(newBudget.Camp);

                if (newBudget.Camp.Id == 0)
                {
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
                _logger.LogError(ex, "Error in UpsertCamp");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
                TempData.Put("ToastMsg", toast);
                newBudget.AvailableTemplates = (await _templateBudgetRepo.GetAll()).ToList();
                return View(nameof(NewBudget), newBudget);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateBudget(CreateBudgetViewModel vm)
        {
            var toast = new ToastMessageViewModel();

            if (!ModelState.IsValid)
            {
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Bitte Vorlage auswählen und Budgetname eingeben.",
                    Type = ToastType.Error
                };
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(NewBudget), new { campId = vm.CampId });
            }

            try
            {
                var budget = new BudgetModel
                {
                    Name = vm.BudgetName,
                    Description = vm.BudgetDescription,
                    CampId = vm.CampId
                };
                int budgetId = await _budgetRepo.Create(budget);

                if (vm.SelectedTemplateBudgetId > 0)
                {
                    var templatePositions = await _templatePositionRepo.GetByTemplateBudgetId(vm.SelectedTemplateBudgetId);
                    foreach (var tp in templatePositions)
                    {
                        var position = new PositionModel
                        {
                            BudgetId = budgetId,
                            PositionTypeId = tp.PositionTypeId,
                            CategoryId = tp.CategoryId,
                            SubCategoryId = tp.SubCategoryId == 0 ? null : tp.SubCategoryId,
                            Name = tp.Name,
                            FixedAmount_fc = tp.FixedAmount ?? 0,
                            Quantity_fc = tp.Quantity ?? 0,
                            UnitAmount_fc = tp.UnitAmount ?? 0,
                            SortIndex = tp.SortIndex
                        };
                        await _positionRepo.Create(position);
                    }
                }

                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Budget erstellt.",
                    Type = ToastType.Success
                };
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(Detail), new { id = budgetId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in CreateBudget");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(NewBudget), new { campId = vm.CampId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var budget = await _budgetRepo.GetById(id);
            if (budget == null) return NotFound();

            var camp = await _campRepo.GetById(budget.CampId);
            var positions = (await _positionRepo.GetByBudgetId(id)).ToList();
            var categories = (await _categoryRepo.GetAll()).ToDictionary(c => c.Id);
            var subCategories = (await _subCategoryRepo.GetAll()).ToDictionary(sc => sc.Id);
            var positionTypes = (await _positionTypeRepo.GetAll()).ToDictionary(pt => pt.Id);

            var groups = positions
                .GroupBy(p => p.CategoryId)
                .Select(g =>
                {
                    categories.TryGetValue(g.Key, out var category);
                    return new CategoryGroupViewModel
                    {
                        Category = category!,
                        SubGroups = g
                            .GroupBy(p => p.SubCategoryId)
                            .Select(sg =>
                            {
                                SubCategoryModel? subCategory = null;
                                if (sg.Key.HasValue)
                                    subCategories.TryGetValue(sg.Key.Value, out subCategory);
                                return new SubCategoryGroupViewModel
                                {
                                    SubCategory = subCategory,
                                    Positions = sg
                                        .OrderBy(p => p.SortIndex)
                                        .Select(p =>
                                        {
                                            positionTypes.TryGetValue(p.PositionTypeId, out var pt);
                                            return new PositionRowViewModel
                                            {
                                                Id = p.Id,
                                                Name = p.Name,
                                                PositionTypeName = pt?.Name ?? string.Empty,
                                                FixedAmount_fc = p.FixedAmount_fc,
                                                Quantity_fc = p.Quantity_fc,
                                                UnitAmount_fc = p.UnitAmount_fc,
                                                FixedAmount_rl = p.FixedAmount_rl,
                                                Quantity_rl = p.Quantity_rl,
                                                UnitAmount_rl = p.UnitAmount_rl
                                            };
                                        })
                                        .ToList()
                                };
                            })
                            .OrderBy(sg => sg.SubCategory?.SortIndex ?? int.MaxValue)
                            .ToList()
                    };
                })
                .OrderBy(g => categories.TryGetValue(g.Category.Id, out var cat) ? cat.SortIndex : 0)
                .ToList();

            var vm = new BudgetDetailViewModel
            {
                Budget = budget,
                Camp = camp!,
                Groups = groups,
                PositionTypes = positionTypes.Values.OrderBy(pt => pt.Name).ToList(),
                AllCategories = categories.Values.OrderBy(c => c.SortIndex).ToList(),
                AllSubCategories = subCategories.Values.OrderBy(sc => sc.SortIndex).ToList()
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRealAmounts(int budgetId, List<PositionUpdateViewModel> positions)
        {
            var toast = new ToastMessageViewModel();

            try
            {
                var currentPositions = (await _positionRepo.GetByBudgetId(budgetId))
                    .ToDictionary(p => p.Id);

                foreach (var update in positions)
                {
                    if (currentPositions.TryGetValue(update.Id, out var pos))
                    {
                        pos.FixedAmount_rl = update.FixedAmount_rl;
                        pos.Quantity_rl = update.Quantity_rl;
                        pos.UnitAmount_rl = update.UnitAmount_rl;
                        await _positionRepo.Update(pos);
                    }
                }

                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Änderungen gespeichert.",
                    Type = ToastType.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SaveRealAmounts");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
            }

            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(Detail), new { id = budgetId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddBudgetPosition(AddBudgetPositionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Bitte alle Pflichtfelder ausfüllen.",
                    Type = ToastType.Error
                };
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(Detail), new { id = vm.BudgetId });
            }

            try
            {
                var existing = await _positionRepo.GetByBudgetId(vm.BudgetId);
                int nextSortIndex = (existing.Any() ? existing.Max(p => p.SortIndex) : 0) + 1;

                var position = new PositionModel
                {
                    BudgetId = vm.BudgetId,
                    PositionTypeId = vm.PositionTypeId,
                    CategoryId = vm.CategoryId,
                    SubCategoryId = vm.SubCategoryId == 0 ? null : vm.SubCategoryId,
                    Name = vm.Name,
                    FixedAmount_fc = vm.FixedAmount,
                    Quantity_fc = vm.Quantity,
                    UnitAmount_fc = vm.UnitAmount,
                    SortIndex = nextSortIndex
                };
                await _positionRepo.Create(position);
                var toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Position hinzugefügt.",
                    Type = ToastType.Success
                };
                TempData.Put("ToastMsg", toast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddBudgetPosition");
                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
                TempData.Put("ToastMsg", toast);
            }
            return RedirectToAction(nameof(Detail), new { id = vm.BudgetId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteBudgetPosition(int id, int budgetId)
        {
            var toast = new ToastMessageViewModel();
            try
            {
                await _positionRepo.Delete(id);
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Position gelöscht.",
                    Type = ToastType.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteBudgetPosition");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(Detail), new { id = budgetId });
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
