using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class TemplateBudgetDetailViewModel
    {
        public required TemplateBudgetModel TemplateBudget { get; set; }
        public List<TemplatePositionRowViewModel> Positions { get; set; } = [];
        public UpsertTemplatePositionViewModel NewPosition { get; set; } = new();
    }

    public class TemplatePositionRowViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string PositionTypeName { get; set; }
        public required string CategoryName { get; set; }
        public required string SubCategoryName { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitAmount { get; set; }
        public int SortIndex { get; set; }
    }

    public class UpsertTemplatePositionViewModel
    {
        public int Id { get; set; }
        public int TemplateBudgetId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PositionTypeId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SubCategoryId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Range(0, double.MaxValue)]
        public decimal? FixedAmount { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal? UnitAmount { get; set; }

        [Range(0, int.MaxValue)]
        public int SortIndex { get; set; }

        public List<PositionTypeModel> PositionTypes { get; set; } = [];
        public List<CategoryModel> Categories { get; set; } = [];
        public List<SubCategoryModel> SubCategories { get; set; } = [];
    }
}
