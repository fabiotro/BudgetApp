using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class BudgetUserModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int BudgetId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        public bool IsMainLeader { get; set; }

        // Joined from User table for display
        public string? DisplayName { get; set; }
        public string? Email { get; set; }
    }
}
