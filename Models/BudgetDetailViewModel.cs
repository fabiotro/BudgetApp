namespace BudgetApp.Models
{
    public class BudgetDetailViewModel
    {
        public required BudgetModel Budget { get; set; }
        public required CampModel Camp { get; set; }
        public List<CategoryGroupViewModel> Groups { get; set; } = [];

        public decimal TotalAmount_fc => Groups.Sum(g => g.TotalAmount_fc);
        public decimal TotalAmount_rl => Groups.Sum(g => g.TotalAmount_rl);
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
        public required SubCategoryModel SubCategory { get; set; }
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
}
