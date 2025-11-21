using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int SortIndex { get; set; }
    }
}
