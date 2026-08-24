using BudgetApp.Data.Repositories;
using BudgetApp.Enums;
using BudgetApp.Extensions;
using BudgetApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.Controllers
{
    public class TemplateBudgetController : Controller
    {
        private readonly ILogger<TemplateBudgetController> _logger;
        private readonly ITemplateBudgetRepository<TemplateBudgetModel> _templateBudgetRepo;
        private readonly ITemplatePositionRepository<TemplatePositionModel> _templatePositionRepo;
        private readonly IPositionTypeRepository<PositionTypeModel> _positionTypeRepo;
        private readonly ICategoryRepository<CategoryModel> _categoryRepo;
        private readonly ISubCategoryRepository<SubCategoryModel> _subCategoryRepo;

        public TemplateBudgetController(
            ILogger<TemplateBudgetController> logger,
            ITemplateBudgetRepository<TemplateBudgetModel> templateBudgetRepo,
            ITemplatePositionRepository<TemplatePositionModel> templatePositionRepo,
            IPositionTypeRepository<PositionTypeModel> positionTypeRepo,
            ICategoryRepository<CategoryModel> categoryRepo,
            ISubCategoryRepository<SubCategoryModel> subCategoryRepo
        )
        {
            _logger = logger;
            _templateBudgetRepo = templateBudgetRepo;
            _templatePositionRepo = templatePositionRepo;
            _positionTypeRepo = positionTypeRepo;
            _categoryRepo = categoryRepo;
            _subCategoryRepo = subCategoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var templates = (await _templateBudgetRepo.GetAll()).ToList();
            return View(templates);
        }

        [HttpGet]
        public async Task<IActionResult> Upsert(int? id)
        {
            if (id.HasValue)
            {
                var template = await _templateBudgetRepo.GetById(id.Value);
                if (template == null)
                    return NotFound();
                return View(template);
            }
            return View(new TemplateBudgetModel { Name = string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(TemplateBudgetModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var toast = new ToastMessageViewModel();
            try
            {
                if (model.Id == 0)
                {
                    int newId = await _templateBudgetRepo.Create(model);
                    toast = new ToastMessageViewModel
                    {
                        Title = "Erfolg",
                        Message = "Vorlage erstellt.",
                        Type = ToastType.Success,
                    };
                    TempData.Put("ToastMsg", toast);
                    return RedirectToAction(nameof(Detail), new { id = newId });
                }
                else
                {
                    await _templateBudgetRepo.Update(model);
                    toast = new ToastMessageViewModel
                    {
                        Title = "Erfolg",
                        Message = "Vorlage aktualisiert.",
                        Type = ToastType.Success,
                    };
                    TempData.Put("ToastMsg", toast);
                    return RedirectToAction(nameof(Detail), new { id = model.Id });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in TemplateBudget Upsert");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
                TempData.Put("ToastMsg", toast);
                return View(model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var template = await _templateBudgetRepo.GetById(id);
            if (template == null)
                return NotFound();

            var positions = (await _templatePositionRepo.GetByTemplateBudgetId(id)).ToList();
            var positionTypes = (await _positionTypeRepo.GetAll()).ToDictionary(pt => pt.Id);
            var categories = (await _categoryRepo.GetAll()).ToDictionary(c => c.Id);
            var subCategories = (await _subCategoryRepo.GetAll()).ToDictionary(sc => sc.Id);

            var rows = positions
                .OrderBy(p => p.SortIndex)
                .Select(p =>
                {
                    positionTypes.TryGetValue(p.PositionTypeId, out var pt);
                    return new TemplatePositionRowViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        PositionTypeName = pt?.Name ?? "–",
                        CategoryId = p.CategoryId,
                        SubCategoryId = p.SubCategoryId,
                        FixedAmount = p.FixedAmount,
                        Quantity = p.Quantity,
                        UnitAmount = p.UnitAmount,
                        SortIndex = p.SortIndex,
                    };
                })
                .ToList();

            var groups = rows.GroupBy(p => p.CategoryId)
                .Select(catGrp =>
                {
                    if (!categories.TryGetValue(catGrp.Key, out var cat))
                        return null;
                    var subGroups = catGrp
                        .GroupBy(p => p.SubCategoryId)
                        .Select(subGrp =>
                        {
                            SubCategoryModel? sub = null;
                            if (subGrp.Key.HasValue)
                                subCategories.TryGetValue(subGrp.Key.Value, out sub);
                            return new TemplateSubCategoryGroupViewModel
                            {
                                SubCategory = sub,
                                Positions = subGrp.OrderBy(p => p.SortIndex).ToList(),
                            };
                        })
                        .OrderBy(sg => sg.SubCategory?.SortIndex ?? -1)
                        .ToList();
                    return new TemplateCategoryGroupViewModel
                    {
                        Category = cat,
                        SubGroups = subGroups,
                    };
                })
                .Where(g => g != null)
                .Cast<TemplateCategoryGroupViewModel>()
                .OrderBy(g => g.Category.SortIndex)
                .ToList();

            var vm = new TemplateBudgetDetailViewModel
            {
                TemplateBudget = template,
                Groups = groups,
                NewPosition = new UpsertTemplatePositionViewModel
                {
                    TemplateBudgetId = id,
                    Name = string.Empty,
                    PositionTypes = positionTypes.Values.OrderBy(pt => pt.Name).ToList(),
                    Categories = categories.Values.OrderBy(c => c.SortIndex).ToList(),
                    SubCategories = subCategories.Values.OrderBy(sc => sc.SortIndex).ToList(),
                },
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTemplate(int id)
        {
            var toast = new ToastMessageViewModel();
            try
            {
                var positions = await _templatePositionRepo.GetByTemplateBudgetId(id);
                foreach (var pos in positions)
                    await _templatePositionRepo.Delete(pos.Id);

                await _templateBudgetRepo.Delete(id);
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Vorlage gelöscht.",
                    Type = ToastType.Success,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteTemplate");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddPosition(UpsertTemplatePositionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Bitte alle Pflichtfelder ausfüllen.",
                    Type = ToastType.Error,
                };
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(Detail), new { id = vm.TemplateBudgetId });
            }

            try
            {
                var existing = await _templatePositionRepo.GetByTemplateBudgetId(
                    vm.TemplateBudgetId
                );
                int nextSortIndex = (existing.Any() ? existing.Max(p => p.SortIndex) : 0) + 1;

                var model = new TemplatePositionModel
                {
                    TemplateBudgetId = vm.TemplateBudgetId,
                    PositionTypeId = vm.PositionTypeId,
                    CategoryId = vm.CategoryId,
                    SubCategoryId = vm.SubCategoryId == 0 ? null : vm.SubCategoryId,
                    Name = vm.Name,
                    FixedAmount = vm.FixedAmount,
                    Quantity = vm.Quantity,
                    UnitAmount = vm.UnitAmount,
                    SortIndex = nextSortIndex,
                };
                await _templatePositionRepo.Create(model);
                var toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Position hinzugefügt.",
                    Type = ToastType.Success,
                };
                TempData.Put("ToastMsg", toast);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddPosition");
                var toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
                TempData.Put("ToastMsg", toast);
            }
            return RedirectToAction(nameof(Detail), new { id = vm.TemplateBudgetId });
        }

        [HttpGet]
        public async Task<IActionResult> EditPositionModal(int id)
        {
            var pos = await _templatePositionRepo.GetById(id);
            if (pos == null)
                return NotFound();

            var positionTypes = (await _positionTypeRepo.GetAll()).OrderBy(pt => pt.Name).ToList();
            var categories = (await _categoryRepo.GetAll()).OrderBy(c => c.SortIndex).ToList();
            var subCategories = (await _subCategoryRepo.GetAll())
                .OrderBy(sc => sc.SortIndex)
                .ToList();

            var vm = new UpsertTemplatePositionViewModel
            {
                Id = pos.Id,
                TemplateBudgetId = pos.TemplateBudgetId,
                PositionTypeId = pos.PositionTypeId,
                CategoryId = pos.CategoryId,
                SubCategoryId = pos.SubCategoryId,
                Name = pos.Name,
                FixedAmount = pos.FixedAmount,
                Quantity = pos.Quantity,
                UnitAmount = pos.UnitAmount,
                SortIndex = pos.SortIndex,
                PositionTypes = positionTypes,
                Categories = categories,
                SubCategories = subCategories,
            };
            return PartialView("_EditPositionModal", vm);
        }

        [HttpGet]
        public async Task<IActionResult> EditPosition(int id)
        {
            var pos = await _templatePositionRepo.GetById(id);
            if (pos == null)
                return NotFound();

            var positionTypes = (await _positionTypeRepo.GetAll()).OrderBy(pt => pt.Name).ToList();
            var categories = (await _categoryRepo.GetAll()).OrderBy(c => c.SortIndex).ToList();
            var subCategories = (await _subCategoryRepo.GetAll())
                .OrderBy(sc => sc.SortIndex)
                .ToList();

            var vm = new UpsertTemplatePositionViewModel
            {
                Id = pos.Id,
                TemplateBudgetId = pos.TemplateBudgetId,
                PositionTypeId = pos.PositionTypeId,
                CategoryId = pos.CategoryId,
                SubCategoryId = pos.SubCategoryId,
                Name = pos.Name,
                FixedAmount = pos.FixedAmount,
                Quantity = pos.Quantity,
                UnitAmount = pos.UnitAmount,
                SortIndex = pos.SortIndex,
                PositionTypes = positionTypes,
                Categories = categories,
                SubCategories = subCategories,
            };
            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SavePosition(UpsertTemplatePositionViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.PositionTypes = (await _positionTypeRepo.GetAll())
                    .OrderBy(pt => pt.Name)
                    .ToList();
                vm.Categories = (await _categoryRepo.GetAll()).OrderBy(c => c.SortIndex).ToList();
                vm.SubCategories = (await _subCategoryRepo.GetAll())
                    .OrderBy(sc => sc.SortIndex)
                    .ToList();
                return View(nameof(EditPosition), vm);
            }

            var toast = new ToastMessageViewModel();
            try
            {
                var model = new TemplatePositionModel
                {
                    Id = vm.Id,
                    TemplateBudgetId = vm.TemplateBudgetId,
                    PositionTypeId = vm.PositionTypeId,
                    CategoryId = vm.CategoryId,
                    SubCategoryId = vm.SubCategoryId == 0 ? null : vm.SubCategoryId,
                    Name = vm.Name,
                    FixedAmount = vm.FixedAmount,
                    Quantity = vm.Quantity,
                    UnitAmount = vm.UnitAmount,
                    SortIndex = vm.SortIndex,
                };
                await _templatePositionRepo.Update(model);
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Position gespeichert.",
                    Type = ToastType.Success,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SavePosition");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(Detail), new { id = vm.TemplateBudgetId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePosition(int id, int templateBudgetId)
        {
            var toast = new ToastMessageViewModel();
            try
            {
                await _templatePositionRepo.Delete(id);
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Position gelöscht.",
                    Type = ToastType.Success,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeletePosition");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(Detail), new { id = templateBudgetId });
        }
    }
}
