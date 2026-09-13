using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class SendInviteViewModel
    {
        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(Resources.DataAnnotations)
        )]
        [EmailAddress(
            ErrorMessageResourceName = "EmailAddress",
            ErrorMessageResourceType = typeof(Resources.DataAnnotations)
        )]
        [Display(Name = "InviteEmail", ResourceType = typeof(Resources.DataAnnotations))]
        public required string Email { get; set; }

        public int BudgetId { get; set; }
    }
}
