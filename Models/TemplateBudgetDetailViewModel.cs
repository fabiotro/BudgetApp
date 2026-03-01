using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class TemplateBudgetDetailViewModel
    {
        public required TemplateBudgetModel TemplateBudget { get; set; }
        public List<TemplateCategoryGroupViewModel> Groups { get; set; } = [];
        public UpsertTemplatePositionViewModel NewPosition { get; set; } = new();

        public decimal TotalAmount   => Groups.Sum(g => g.TotalAmount);
        public decimal TotalIncome   => Groups.SelectMany(g => g.SubGroups).SelectMany(sg => sg.Positions).Where(p => p.IsIncome).Sum(p => p.TotalAmount);
        public decimal TotalExpenses => Groups.SelectMany(g => g.SubGroups).SelectMany(sg => sg.Positions).Where(p => !p.IsIncome).Sum(p => p.TotalAmount);
    }

    public class TemplateCategoryGroupViewModel
    {
        public required CategoryModel Category { get; set; }
        public List<TemplateSubCategoryGroupViewModel> SubGroups { get; set; } = [];

        public decimal TotalAmount => SubGroups.Sum(g => g.TotalAmount);
    }

    public class TemplateSubCategoryGroupViewModel
    {
        public SubCategoryModel? SubCategory { get; set; }
        public List<TemplatePositionRowViewModel> Positions { get; set; } = [];

        public decimal TotalAmount => Positions.Sum(p => p.SignedTotalAmount);
    }

    public class TemplatePositionRowViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string PositionTypeName { get; set; }
        public bool IsIncome => PositionTypeName == "Einnahme";
        public int CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public decimal? FixedAmount { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitAmount { get; set; }
        public int SortIndex { get; set; }

        public decimal TotalAmount       => (FixedAmount ?? 0) + ((Quantity ?? 0) * (UnitAmount ?? 0));
        public decimal SignedTotalAmount  => IsIncome ? TotalAmount : -TotalAmount;
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

        public int? SubCategoryId { get; set; }

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
