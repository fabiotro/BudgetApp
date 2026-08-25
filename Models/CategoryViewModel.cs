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

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Range(
            1,
            int.MaxValue,
            ErrorMessageResourceName = "Range",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "SubCategoryCategoryId", ResourceType = typeof(DataAnnotations))]
        public int CategoryId { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [StringLength(
            100,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "SubCategoryName", ResourceType = typeof(DataAnnotations))]
        public string Name { get; set; } = string.Empty;

        [StringLength(
            255,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
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
