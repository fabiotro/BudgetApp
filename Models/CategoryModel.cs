using System.ComponentModel.DataAnnotations;
using BudgetApp.Resources;

namespace BudgetApp.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "CategoryName", ResourceType = typeof(DataAnnotations))]
        public required string Name { get; set; }

        [StringLength(255)]
        [Display(Name = "CategoryDescription", ResourceType = typeof(DataAnnotations))]
        public string? Description { get; set; }

        public int SortIndex { get; set; }
    }
}
