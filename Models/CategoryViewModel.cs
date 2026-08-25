using System.ComponentModel.DataAnnotations;
using BudgetApp.Resources;

namespace BudgetApp.Models
{
    public class CategoryIndexViewModel
    {
        public List<CategoryWithSubsViewModel> Categories { get; set; } = [];
    }

    public class CategoryWithSubsViewModel
    {
        public required CategoryModel Category { get; set; }
        public List<SubCategoryModel> SubCategories { get; set; } = [];
    }

    public class UpsertSubCategoryViewModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        [Display(Name = "SubCategoryCategoryId", ResourceType = typeof(DataAnnotations))]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "SubCategoryName", ResourceType = typeof(DataAnnotations))]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        [Display(Name = "SubCategoryDescription", ResourceType = typeof(DataAnnotations))]
        public string? Description { get; set; }

        public List<CategoryModel> Categories { get; set; } = [];
    }

    public class ReorderItemViewModel
    {
        public int Id { get; set; }
        public int SortIndex { get; set; }
    }

    public class ReorderSubCategoriesViewModel
    {
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }
        public List<ReorderItemViewModel> Items { get; set; } = [];
    }
}
