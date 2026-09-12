using System.ComponentModel.DataAnnotations;
using BudgetApp.Enums;

namespace BudgetApp.Models
{
    public class TransactionModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CampId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int PerformedByUserId { get; set; }

        public int? PositionId { get; set; }

        [Required]
        [StringLength(255)]
        public required string Name { get; set; }

        [StringLength(500)]
        public string? Description { get; set; }

        [Required]
        [Range(0, double.MaxValue)]
        public decimal Amount { get; set; }

        [Required]
        public PaymentSource PaymentSource { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }
    }
}
