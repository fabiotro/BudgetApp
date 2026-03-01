using System.ComponentModel.DataAnnotations;

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
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [StringLength(255)]
        public string? Description { get; set; }

        [Range(0, int.MaxValue)]
        public int SortIndex { get; set; }

        public List<CategoryModel> Categories { get; set; } = [];
    }
}
