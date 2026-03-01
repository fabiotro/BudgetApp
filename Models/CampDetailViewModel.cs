namespace BudgetApp.Models
{
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

    public class CampDetailViewModel
    {
        public required CampModel Camp { get; set; }
        public List<UserExpenseSummaryViewModel> UserSummaries { get; set; } = [];
        public List<BudgetModel> Budgets { get; set; } = [];
    }
}
