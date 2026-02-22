using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class BudgetDetailViewModel
    {
        public required BudgetModel Budget { get; set; }
        public required CampModel Camp { get; set; }
        public List<CategoryGroupViewModel> Groups { get; set; } = [];

        public decimal TotalAmount_fc => Groups.Sum(g => g.TotalAmount_fc);
        public decimal TotalAmount_rl => Groups.Sum(g => g.TotalAmount_rl);

        public List<PositionTypeModel> PositionTypes { get; set; } = [];
        public List<CategoryModel> AllCategories { get; set; } = [];
        public List<SubCategoryModel> AllSubCategories { get; set; } = [];
    }

    public class CategoryGroupViewModel
    {
        public required CategoryModel Category { get; set; }
        public List<SubCategoryGroupViewModel> SubGroups { get; set; } = [];

        public decimal TotalAmount_fc => SubGroups.Sum(g => g.TotalAmount_fc);
        public decimal TotalAmount_rl => SubGroups.Sum(g => g.TotalAmount_rl);
    }

    public class SubCategoryGroupViewModel
    {
        public SubCategoryModel? SubCategory { get; set; }
        public List<PositionRowViewModel> Positions { get; set; } = [];

        public decimal TotalAmount_fc => Positions.Sum(p => p.TotalAmount_fc);
        public decimal TotalAmount_rl => Positions.Sum(p => p.TotalAmount_rl);
    }

    public class PositionRowViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public decimal FixedAmount_fc { get; set; }
        public decimal Quantity_fc { get; set; }
        public decimal UnitAmount_fc { get; set; }
        public decimal? FixedAmount_rl { get; set; }
        public decimal? Quantity_rl { get; set; }
        public decimal? UnitAmount_rl { get; set; }

        public decimal TotalAmount_fc => FixedAmount_fc + (Quantity_fc * UnitAmount_fc);
        public decimal TotalAmount_rl => (FixedAmount_rl ?? 0) + ((Quantity_rl ?? 0) * (UnitAmount_rl ?? 0));
    }

    public class PositionUpdateViewModel
    {
        public int Id { get; set; }
        public decimal? FixedAmount_rl { get; set; }
        public decimal? Quantity_rl { get; set; }
        public decimal? UnitAmount_rl { get; set; }
    }

    public class AddBudgetPositionViewModel
    {
        public int BudgetId { get; set; }

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
        public decimal FixedAmount_fc { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Quantity_fc { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitAmount_fc { get; set; }
    }
}
