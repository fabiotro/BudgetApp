using BudgetApp.Resources;
using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class TemplateBudgetModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CreatedByUserId { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [StringLength(100, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "TemplateBudgetName", ResourceType = typeof(DataAnnotations))]
        public required string Name { get; set; }

        [StringLength(255, ErrorMessageResourceName = "StringLength", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "TemplateBudgetDescription", ResourceType = typeof(DataAnnotations))]
        public string? Description { get; set; }
    }
}
