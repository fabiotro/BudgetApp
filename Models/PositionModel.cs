using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class PositionModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int BudgetId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PositionTypeId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CategoryId { get; set; }

        public int? SubCategoryId { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public decimal FixedAmount_fc { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public decimal Quantity_fc { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public decimal UnitAmount_fc { get; set; }

        [Range(0, int.MaxValue)]
        public decimal? FixedAmount_rl { get; set; }

        [Range(0, int.MaxValue)]
        public decimal? Quantity_rl { get; set; }

        [Range(0, int.MaxValue)]
        public decimal? UnitAmount_rl { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int SortIndex { get; set; }
    }
}
