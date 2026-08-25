using System.ComponentModel.DataAnnotations;
using BudgetApp.Resources;

namespace BudgetApp.Models
{
    public class SubCategoryModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "SubCategoryName", ResourceType = typeof(DataAnnotations))]
        public required string Name { get; set; }

        [StringLength(255)]
        [Display(Name = "SubCategoryDescription", ResourceType = typeof(DataAnnotations))]
        public string? Description { get; set; }

        public int SortIndex { get; set; }
    }
}
