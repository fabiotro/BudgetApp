using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class SendInviteViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "InviteEmail", ResourceType = typeof(Resources.DataAnnotations))]
        public required string Email { get; set; }

        public int BudgetId { get; set; }
    }
}
