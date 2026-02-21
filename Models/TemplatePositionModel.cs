using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class TemplatePositionModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TemplateBudgetId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PositionTypeId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int SubCategoryId { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [Range(0, int.MaxValue)]
        public decimal? FixedAmount { get; set; }

        [Range(0, int.MaxValue)]
        public decimal? Quantity { get; set; }

        [Range(0, int.MaxValue)]
        public decimal? UnitAmount { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int SortIndex { get; set; }
    }
}
