using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class PositionTypeModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        [StringLength(255)]
        public required string Description { get; set; }
    }
}
