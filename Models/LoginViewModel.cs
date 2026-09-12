using System.ComponentModel.DataAnnotations;
using BudgetApp.Resources;

namespace BudgetApp.Models
{
    public class LoginViewModel
    {
        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [EmailAddress]
        [Display(Name = "LoginEmail", ResourceType = typeof(DataAnnotations))]
        public required string Email { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [DataType(DataType.Password)]
        [Display(Name = "LoginPassword", ResourceType = typeof(DataAnnotations))]
        public required string Password { get; set; }
    }
}
