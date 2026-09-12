using System.ComponentModel.DataAnnotations;
using BudgetApp.Enums;

namespace BudgetApp.Models
{
    public class BudgetInviteModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int BudgetId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int InvitedByUserId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int InvitedUserId { get; set; }

        [Required]
        public InviteStatus Status { get; set; }

        // Joined from User table for display
        public string? InvitedByDisplayName { get; set; }
        public string? InvitedUserDisplayName { get; set; }
        public string? InvitedUserEmail { get; set; }

        // Joined from Budget table for display
        public string? BudgetName { get; set; }
    }
}
