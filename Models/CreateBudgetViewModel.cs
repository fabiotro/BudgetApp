using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class CreateBudgetViewModel
    {
        public int CampId { get; set; }

        public int SelectedTemplateBudgetId { get; set; }

        [Required]
        [StringLength(100)]
        public required string BudgetName { get; set; }

        [StringLength(255)]
        public string? BudgetDescription { get; set; }
    }
}
