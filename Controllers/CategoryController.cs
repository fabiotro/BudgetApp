using BudgetApp.Data.Repositories;
using BudgetApp.Enums;
using BudgetApp.Extensions;
using BudgetApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApp.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepository<CategoryModel> _categoryRepo;
        private readonly ISubCategoryRepository<SubCategoryModel> _subCategoryRepo;

        public CategoryController(
            ILogger<CategoryController> logger,
            ICategoryRepository<CategoryModel> categoryRepo,
            ISubCategoryRepository<SubCategoryModel> subCategoryRepo
        )
        {
            _logger = logger;
            _categoryRepo = categoryRepo;
            _subCategoryRepo = subCategoryRepo;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var categories = (await _categoryRepo.GetAll()).OrderBy(c => c.SortIndex).ToList();
            var subCategories = (await _subCategoryRepo.GetAll())
                .OrderBy(sc => sc.SortIndex)
                .ToList();

            var subsByCategoryId = subCategories
                .GroupBy(sc => sc.CategoryId)
                .ToDictionary(g => g.Key, g => g.ToList());

            var vm = new CategoryIndexViewModel
            {
                Categories = categories
                    .Select(c => new CategoryWithSubsViewModel
                    {
                        Category = c,
                        SubCategories = subsByCategoryId.TryGetValue(c.Id, out var subs)
                            ? subs
                            : [],
                    })
                    .ToList(),
            };

            return View(vm);
        }

        [HttpGet]
        public async Task<IActionResult> UpsertCategory(int? id)
        {
            if (id.HasValue)
            {
                var category = await _categoryRepo.GetById(id.Value);
                if (category == null)
                    return NotFound();
                return View(category);
            }
            return View(new CategoryModel { Name = string.Empty });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertCategory(CategoryModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var toast = new ToastMessageViewModel();
            try
            {
                if (model.Id == 0)
                {
                    var existingCategories = await _categoryRepo.GetAll();
                    model.SortIndex = existingCategories.Any()
                        ? existingCategories.Max(c => c.SortIndex) + 1
                        : 1;
                    await _categoryRepo.Create(model);
                    toast = new ToastMessageViewModel
                    {
                        Title = "Erfolg",
                        Message = "Kategorie erstellt.",
                        Type = ToastType.Success,
                    };
                }
                else
                {
                    await _categoryRepo.Update(model);
                    toast = new ToastMessageViewModel
                    {
                        Title = "Erfolg",
                        Message = "Kategorie aktualisiert.",
                        Type = ToastType.Success,
                    };
                }
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in Category UpsertCategory");
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var toast = new ToastMessageViewModel();
            try
            {
                await _categoryRepo.Delete(id);
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Kategorie gelöscht.",
                    Type = ToastType.Success,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteCategory");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message =
                        "Kategorie konnte nicht gelöscht werden. Möglicherweise sind noch Unterkategorien oder Positionen vorhanden.",
                    Type = ToastType.Error,
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(Index));
        }

        [HttpGet]
        public async Task<IActionResult> UpsertSubCategory(int? id, int? categoryId)
        {
            var categories = (await _categoryRepo.GetAll()).OrderBy(c => c.SortIndex).ToList();

            if (id.HasValue)
            {
                var sub = await _subCategoryRepo.GetById(id.Value);
                if (sub == null)
                    return NotFound();

                var vm = new UpsertSubCategoryViewModel
                {
                    Id = sub.Id,
                    CategoryId = sub.CategoryId,
                    Name = sub.Name,
                    Description = sub.Description,
                    Categories = categories,
                };
                return View(vm);
            }
            else
            {
                var vm = new UpsertSubCategoryViewModel
                {
                    CategoryId = categoryId ?? 0,
                    Categories = categories,
                };
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpsertSubCategory(UpsertSubCategoryViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Categories = (await _categoryRepo.GetAll()).OrderBy(c => c.SortIndex).ToList();
                return View(vm);
            }

            var toast = new ToastMessageViewModel();
            try
            {
                var model = new SubCategoryModel
                {
                    Id = vm.Id,
                    CategoryId = vm.CategoryId,
                    Name = vm.Name,
                    Description = vm.Description,
                };

                if (vm.Id == 0)
                {
                    var existingSubs = await _subCategoryRepo.GetAll();
                    var siblings = existingSubs
                        .Where(sc => sc.CategoryId == vm.CategoryId)
                        .ToList();
                    model.SortIndex = siblings.Any() ? siblings.Max(sc => sc.SortIndex) + 1 : 1;
                    await _subCategoryRepo.Create(model);
                    toast = new ToastMessageViewModel
                    {
                        Title = "Erfolg",
                        Message = "Unterkategorie erstellt.",
                        Type = ToastType.Success,
                    };
                }
                else
                {
                    await _subCategoryRepo.Update(model);
                    toast = new ToastMessageViewModel
                    {
                        Title = "Erfolg",
                        Message = "Unterkategorie aktualisiert.",
                        Type = ToastType.Success,
                    };
                }
                TempData.Put("ToastMsg", toast);
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpsertSubCategory");
                vm.Categories = (await _categoryRepo.GetAll()).OrderBy(c => c.SortIndex).ToList();
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message = "Ein unerwarteter Fehler ist aufgetreten.",
                    Type = ToastType.Error,
                };
                TempData.Put("ToastMsg", toast);
                return View(vm);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteSubCategory(int id)
        {
            var toast = new ToastMessageViewModel();
            try
            {
                await _subCategoryRepo.Delete(id);
                toast = new ToastMessageViewModel
                {
                    Title = "Erfolg",
                    Message = "Unterkategorie gelöscht.",
                    Type = ToastType.Success,
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteSubCategory");
                toast = new ToastMessageViewModel
                {
                    Title = "Fehler",
                    Message =
                        "Unterkategorie konnte nicht gelöscht werden. Möglicherweise sind noch Positionen vorhanden.",
                    Type = ToastType.Error,
                };
            }
            TempData.Put("ToastMsg", toast);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderCategories(List<ReorderItemViewModel> items)
        {
            if (items == null || items.Count == 0)
                return BadRequest();

            try
            {
                await _categoryRepo.UpdateSortOrder(items.Select(i => (i.Id, i.SortIndex)));
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReorderCategories");
                return StatusCode(500);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReorderSubCategories(ReorderSubCategoriesViewModel model)
        {
            if (model?.Items == null || model.Items.Count == 0 || model.CategoryId <= 0)
                return BadRequest();

            try
            {
                await _subCategoryRepo.UpdateSortOrder(
                    model.Items.Select(i => (i.Id, i.SortIndex))
                );
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ReorderSubCategories");
                return StatusCode(500);
            }
        }
    }
}
