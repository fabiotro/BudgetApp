using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class BudgetDetailViewModel
    {
        public required BudgetModel Budget { get; set; }
        public List<BudgetUserModel> BudgetUsers { get; set; } = [];
        public List<CategoryGroupViewModel> Groups { get; set; } = [];

        public decimal TotalAmount_fc => Groups.Sum(g => g.TotalAmount_fc);
        public decimal TotalAmount_rl => Groups.Sum(g => g.TotalAmount_rl);

        public decimal TotalIncome_fc =>
            Groups
                .SelectMany(g => g.SubGroups)
                .SelectMany(sg => sg.Positions)
                .Where(p => p.IsIncome)
                .Sum(p => p.TotalAmount_fc);
        public decimal TotalExpenses_fc =>
            Groups
                .SelectMany(g => g.SubGroups)
                .SelectMany(sg => sg.Positions)
                .Where(p => !p.IsIncome)
                .Sum(p => p.TotalAmount_fc);
        public decimal TotalIncome_rl =>
            Groups
                .SelectMany(g => g.SubGroups)
                .SelectMany(sg => sg.Positions)
                .Where(p => p.IsIncome)
                .Sum(p => p.TotalAmount_rl);
        public decimal TotalExpenses_rl =>
            Groups
                .SelectMany(g => g.SubGroups)
                .SelectMany(sg => sg.Positions)
                .Where(p => !p.IsIncome)
                .Sum(p => p.TotalAmount_rl);

        public List<PositionTypeModel> PositionTypes { get; set; } = [];
        public List<CategoryModel> AllCategories { get; set; } = [];
        public List<SubCategoryModel> AllSubCategories { get; set; } = [];
    }

    public class BudgetLeadersViewModel
    {
        public required BudgetModel Budget { get; set; }
        public List<BudgetUserModel> BudgetUsers { get; set; } = [];
        public List<UserExpenseSummaryViewModel> UserSummaries { get; set; } = [];
        public List<BudgetInviteModel> Invites { get; set; } = [];
        public bool IsMainLeader { get; set; }
        public SendInviteViewModel InviteForm { get; set; } = new() { Email = string.Empty };
    }

    public class UserExpenseSummaryViewModel
    {
        public int UserId { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IBAN { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal PersonalMoneyOwed { get; set; }

        public string FullName =>
            (!string.IsNullOrWhiteSpace(FirstName) || !string.IsNullOrWhiteSpace(LastName))
                ? $"{FirstName} {LastName}".Trim()
                : DisplayName;
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

        public decimal TotalAmount_fc => Positions.Sum(p => p.SignedTotalAmount_fc);
        public decimal TotalAmount_rl => Positions.Sum(p => p.SignedTotalAmount_rl);
    }

    public class PositionRowViewModel
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string PositionTypeName { get; set; } = string.Empty;
        public bool IsIncome => PositionTypeName == "Einnahme";
        public decimal FixedAmount_fc { get; set; }
        public decimal Quantity_fc { get; set; }
        public decimal UnitAmount_fc { get; set; }
        public decimal? FixedAmount_rl { get; set; }
        public decimal? Quantity_rl { get; set; }
        public decimal? UnitAmount_rl { get; set; }
        public string? QuantityVar_fc { get; set; }
        public string? QuantityVar_rl { get; set; }

        public decimal TotalAmount_fc => FixedAmount_fc + (Quantity_fc * UnitAmount_fc);
        public decimal TotalAmount_rl =>
            (FixedAmount_rl ?? 0) + ((Quantity_rl ?? 0) * (UnitAmount_rl ?? 0));

        public decimal SignedTotalAmount_fc => IsIncome ? TotalAmount_fc : -TotalAmount_fc;
        public decimal SignedTotalAmount_rl => IsIncome ? TotalAmount_rl : -TotalAmount_rl;
    }

    public class PositionUpdateViewModel
    {
        public int Id { get; set; }
        public decimal? FixedAmount_rl { get; set; }
        public decimal? Quantity_rl { get; set; }
        public decimal? UnitAmount_rl { get; set; }

        [StringLength(50)]
        public string? QuantityVar_rl { get; set; }
    }

    public class EditBudgetPositionViewModel
    {
        public int Id { get; set; }
        public int BudgetId { get; set; }

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [Range(1, int.MaxValue)]
        public int PositionTypeId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public int? SubCategoryId { get; set; }

        // Provisorisch
        public decimal? FixedAmount_fc { get; set; }

        [StringLength(50)]
        public string? QuantityVar_fc { get; set; }
        public decimal? Quantity_fc { get; set; }
        public decimal? UnitAmount_fc { get; set; }

        // Definitiv
        public decimal? FixedAmount_rl { get; set; }

        [StringLength(50)]
        public string? QuantityVar_rl { get; set; }
        public decimal? Quantity_rl { get; set; }
        public decimal? UnitAmount_rl { get; set; }

        // Select lists (not submitted)
        public List<PositionTypeModel> PositionTypes { get; set; } = [];
        public List<CategoryModel> Categories { get; set; } = [];
        public List<SubCategoryModel> SubCategories { get; set; } = [];

        // Camp variable values for dropdowns
        public int CampParticipantsCount_fc { get; set; }
        public int CampJs_PersonsCount_fc { get; set; }
        public int CampLeadersTeamCount_fc { get; set; }
        public int? CampParticipantsCount_rl { get; set; }
        public int? CampJs_PersonsCount_rl { get; set; }
        public int? CampLeadersTeamCount_rl { get; set; }
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
        public decimal FixedAmount { get; set; }

        [StringLength(50)]
        public string? QuantityVar { get; set; }

        [Range(0, double.MaxValue)]
        public decimal Quantity { get; set; }

        [Range(0, double.MaxValue)]
        public decimal UnitAmount { get; set; }
    }
}
