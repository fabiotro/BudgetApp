namespace BudgetApp.Models
{
    public class BudgetListViewModel
    {
        public required BudgetModel Budget { get; set; }
        public CampModel? Camp { get; set; }
    }
}
