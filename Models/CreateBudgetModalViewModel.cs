namespace BudgetApp.Models
{
    public class CreateBudgetModalViewModel
    {
        public int CampId { get; set; }
        public List<TemplateBudgetModel> AvailableTemplates { get; set; } = [];
    }
}
