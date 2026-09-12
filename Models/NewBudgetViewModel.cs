namespace BudgetApp.Models
{
    public class NewBudgetViewModel
    {
        public CampViewModel Camp { get; set; } = new CampViewModel();

        // Populated by the controller for display; not posted back
        public List<TemplateBudgetModel> AvailableTemplates { get; set; } = [];
        public List<BudgetModel> ExistingBudgets { get; set; } = [];
        public List<CampUserModel> CampUsers { get; set; } = [];
    }
}
