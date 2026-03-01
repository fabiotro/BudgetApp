using BudgetApp.Data.Repositories;
using BudgetApp.Enums;
using BudgetApp.Extensions;
using BudgetApp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BudgetApp.Controllers
{
    [Authorize]
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
        private readonly IUserRepository<UserModel> _userRepo;
        private readonly ICampUserRepository<CampUserModel> _campUserRepo;

        public BudgetController(
            ILogger<BudgetController> logger,
            ICampRepository<CampModel> campRepo,
            IBudgetRepository<BudgetModel> budgetRepo,
            ITemplateBudgetRepository<TemplateBudgetModel> templateBudgetRepo,
            ITemplatePositionRepository<TemplatePositionModel> templatePositionRepo,
            IPositionRepository<PositionModel> positionRepo,
            IPositionTypeRepository<PositionTypeModel> positionTypeRepo,
            ICategoryRepository<CategoryModel> categoryRepo,
            ISubCategoryRepository<SubCategoryModel> subCategoryRepo,
            IUserRepository<UserModel> userRepo,
            ICampUserRepository<CampUserModel> campUserRepo)
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
            _userRepo = userRepo;
            _campUserRepo = campUserRepo;
        }

        private int GetCurrentUserId() =>
            int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            int userId = GetCurrentUserId();
            var budgets = (await _budgetRepo.GetAllForUser(userId)).ToList();
            var camps = (await _campRepo.GetAllForUser(userId)).ToDictionary(c => c.Id);

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
            int userId = GetCurrentUserId();
            var vm = new NewBudgetViewModel
            {
                Camp = new CampViewModel
                {
                    StartDate = DateTime.Today,
                    EndDate = DateTime.Today.AddDays(7)
                },
                AvailableTemplates = (await _templateBudgetRepo.GetAllForUser(userId)).ToList(),
                AllUsers = (await _userRepo.GetAll()).Where(u => u.Id != userId).ToList()
            };

            if (campId.HasValue)
            {
                var camp = await _campRepo.GetById(campId.Value);
                if (camp != null)
                {
                    vm.Camp = MapModelToViewModel(camp);
                    vm.ExistingBudgets = (await _budgetRepo.GetByCampId(campId.Value)).ToList();
                    vm.CampUsers = (await _campUserRepo.GetByCampId(campId.Value)).ToList();
                }
            }

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> CreateBudgetModal(int campId)
        {
            int userId = GetCurrentUserId();
            var vm = new CreateBudgetModalViewModel
            {
                CampId = campId,
                AvailableTemplates = (await _templateBudgetRepo.GetAllForUser(userId)).ToList()
            };
            return PartialView("_CreateBudgetModal", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertCamp(NewBudgetViewModel newBudget)
        {
            int userId = GetCurrentUserId();

            if (!ModelState.IsValid)
            {
                newBudget.AvailableTemplates = (await _templateBudgetRepo.GetAllForUser(userId)).ToList();
                newBudget.AllUsers = (await _userRepo.GetAll()).Where(u => u.Id != userId).ToList();
                if (newBudget.Camp.Id > 0)
                {
                    newBudget.ExistingBudgets = (await _budgetRepo.GetByCampId(newBudget.Camp.Id)).ToList();
                    newBudget.CampUsers = (await _campUserRepo.GetByCampId(newBudget.Camp.Id)).ToList();
                }
                return View(nameof(NewBudget), newBudget);
            }

            var toast = new ToastMessageViewModel();

            try
            {
                var campModel = MapViewModelToModel(newBudget.Camp);

                if (newBudget.Camp.Id == 0)
                {
                    campModel.CreatedByUserId = userId;
                    int newId = await _campRepo.Create(campModel);
                    newBudget.Camp.Id = newId;

                    // Auto-add creator as main leader
                    await _campUserRepo.Create(new CampUserModel
                    {
                        CampId = newId,
                        UserId = userId,
                        IsMainLeader = true
                    });

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
                newBudget.AvailableTemplates = (await _templateBudgetRepo.GetAllForUser(userId)).ToList();
                newBudget.AllUsers = (await _userRepo.GetAll()).Where(u => u.Id != userId).ToList();
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
                            QuantityVar_fc = string.IsNullOrEmpty(tp.QuantityVar) ? null : tp.QuantityVar + "_fc",
                            Quantity_fc = string.IsNullOrEmpty(tp.QuantityVar) ? (tp.Quantity ?? 0) : 0m,
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
            var campUsers = (await _campUserRepo.GetByCampId(budget.CampId)).ToList();
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
                                                Quantity_fc = p.QuantityVar_fc switch
                                                {
                                                    "ParticipantsCount_fc" => (decimal)camp!.ParticipantsCount_fc,
                                                    "js_PersonsCount_fc"   => (decimal)camp!.js_PersonsCount_fc,
                                                    "LeadersTeamCount_fc"  => (decimal)camp!.LeadersTeamCount_fc,
                                                    _                      => p.Quantity_fc
                                                },
                                                UnitAmount_fc = p.UnitAmount_fc,
                                                FixedAmount_rl = p.FixedAmount_rl,
                                                Quantity_rl = p.QuantityVar_rl switch
                                                {
                                                    "ParticipantsCount_rl" => (decimal?)(camp!.ParticipantsCount_rl ?? 0),
                                                    "js_PersonsCount_rl"   => (decimal?)(camp!.js_PersonsCount_rl ?? 0),
                                                    "LeadersTeamCount_rl"  => (decimal?)(camp!.LeadersTeamCount_rl ?? 0),
                                                    _                      => p.Quantity_rl
                                                },
                                                UnitAmount_rl = p.UnitAmount_rl,
                                                QuantityVar_fc = p.QuantityVar_fc,
                                                QuantityVar_rl = p.QuantityVar_rl
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
                CampUsers = campUsers,
                Groups = groups,
                PositionTypes = positionTypes.Values.OrderBy(pt => pt.Name).ToList(),
                AllCategories = categories.Values.OrderBy(c => c.SortIndex).ToList(),
                AllSubCategories = subCategories.Values.OrderBy(sc => sc.SortIndex).ToList()
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> EditPositionModal(int id)
        {
            var pos = await _positionRepo.GetById(id);
            if (pos == null) return NotFound();

            var budget = await _budgetRepo.GetById(pos.BudgetId);
            if (budget == null) return NotFound();

            var camp = await _campRepo.GetById(budget.CampId);
            var positionTypes = (await _positionTypeRepo.GetAll()).OrderBy(pt => pt.Name).ToList();
            var categories = (await _categoryRepo.GetAll()).OrderBy(c => c.SortIndex).ToList();
            var subCategories = (await _subCategoryRepo.GetAll()).OrderBy(sc => sc.SortIndex).ToList();

            var vm = new EditBudgetPositionViewModel
            {
                Id = pos.Id,
                BudgetId = pos.BudgetId,
                Name = pos.Name,
                PositionTypeId = pos.PositionTypeId,
                CategoryId = pos.CategoryId,
                SubCategoryId = pos.SubCategoryId,
                FixedAmount_fc = pos.FixedAmount_fc,
                QuantityVar_fc = pos.QuantityVar_fc,
                Quantity_fc = pos.Quantity_fc,
                UnitAmount_fc = pos.UnitAmount_fc,
                FixedAmount_rl = pos.FixedAmount_rl,
                QuantityVar_rl = pos.QuantityVar_rl,
                Quantity_rl = pos.Quantity_rl,
                UnitAmount_rl = pos.UnitAmount_rl,
                PositionTypes = positionTypes,
                Categories = categories,
                SubCategories = subCategories,
                CampParticipantsCount_fc = camp?.ParticipantsCount_fc ?? 0,
                CampJs_PersonsCount_fc = camp?.js_PersonsCount_fc ?? 0,
                CampLeadersTeamCount_fc = camp?.LeadersTeamCount_fc ?? 0,
                CampParticipantsCount_rl = camp?.ParticipantsCount_rl,
                CampJs_PersonsCount_rl = camp?.js_PersonsCount_rl,
                CampLeadersTeamCount_rl = camp?.LeadersTeamCount_rl
            };
            return PartialView("_EditPositionModal", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePosition(EditBudgetPositionViewModel vm)
        {
            var toast = new ToastMessageViewModel();
            try
            {
                var pos = await _positionRepo.GetById(vm.Id);
                if (pos != null)
                {
                    pos.Name = vm.Name;
                    pos.PositionTypeId = vm.PositionTypeId;
                    pos.CategoryId = vm.CategoryId;
                    pos.SubCategoryId = vm.SubCategoryId == 0 ? null : vm.SubCategoryId;
                    pos.FixedAmount_fc = vm.FixedAmount_fc ?? 0;
                    pos.QuantityVar_fc = string.IsNullOrEmpty(vm.QuantityVar_fc) ? null : vm.QuantityVar_fc;
                    pos.Quantity_fc = string.IsNullOrEmpty(vm.QuantityVar_fc) ? (vm.Quantity_fc ?? 0) : 0m;
                    pos.UnitAmount_fc = vm.UnitAmount_fc ?? 0;
                    pos.FixedAmount_rl = vm.FixedAmount_rl;
                    pos.QuantityVar_rl = string.IsNullOrEmpty(vm.QuantityVar_rl) ? null : vm.QuantityVar_rl;
                    pos.Quantity_rl = string.IsNullOrEmpty(vm.QuantityVar_rl) ? vm.Quantity_rl : 0m;
                    pos.UnitAmount_rl = vm.UnitAmount_rl;
                    await _positionRepo.Update(pos);
                }
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Position gespeichert.",
                    Type = ToastType.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SavePosition");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(Detail), new { id = vm.BudgetId });
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
                    QuantityVar_fc = string.IsNullOrEmpty(vm.QuantityVar) ? null : vm.QuantityVar,
                    Quantity_fc = string.IsNullOrEmpty(vm.QuantityVar) ? vm.Quantity : 0m,
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
        public async Task<IActionResult> DeleteBudget(int id, int campId)
        {
            var toast = new ToastMessageViewModel();
            try
            {
                await _positionRepo.DeleteByBudgetId(id);
                await _budgetRepo.Delete(id);
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Budget gelöscht.",
                    Type = ToastType.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteBudget");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(NewBudget), new { campId });
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCampUser(int campId, int userId)
        {
            var toast = new ToastMessageViewModel();
            try
            {
                var camp = await _campRepo.GetById(campId);
                if (camp == null || camp.CreatedByUserId != GetCurrentUserId())
                {
                    toast = new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Keine Berechtigung.",
                        Type = ToastType.Error
                    };
                    TempData.Put("ToastMsg", toast);
                    return RedirectToAction(nameof(NewBudget), new { campId });
                }

                await _campUserRepo.Create(new CampUserModel
                {
                    CampId = campId,
                    UserId = userId,
                    IsMainLeader = false
                });
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Person hinzugefügt.",
                    Type = ToastType.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddCampUser");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(NewBudget), new { campId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveCampUser(int campUserId, int campId)
        {
            var toast = new ToastMessageViewModel();
            try
            {
                var camp = await _campRepo.GetById(campId);
                if (camp == null || camp.CreatedByUserId != GetCurrentUserId())
                {
                    toast = new ToastMessageViewModel
                    {
                        Title = "Fehler",
                        Message = "Keine Berechtigung.",
                        Type = ToastType.Error
                    };
                    TempData.Put("ToastMsg", toast);
                    return RedirectToAction(nameof(NewBudget), new { campId });
                }

                await _campUserRepo.Delete(campUserId);
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Person entfernt.",
                    Type = ToastType.Success
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RemoveCampUser");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(NewBudget), new { campId });
        }

        #region Helpers
        private CampModel MapViewModelToModel(CampViewModel campViewModel)
        {
            return new CampModel
            {
                Id = campViewModel.Id,
                StartDate = campViewModel.StartDate,
                EndDate = campViewModel.EndDate,
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
