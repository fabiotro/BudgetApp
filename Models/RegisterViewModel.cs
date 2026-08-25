using System.ComponentModel.DataAnnotations;
using BudgetApp.Resources;

namespace BudgetApp.Models
{
    public class RegisterViewModel
    {
        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [EmailAddress]
        [Display(Name = "RegisterEmail", ResourceType = typeof(DataAnnotations))]
        public required string Email { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [StringLength(
            100,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "RegisterDisplayName", ResourceType = typeof(DataAnnotations))]
        public required string DisplayName { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [StringLength(
            100,
            MinimumLength = 8,
            ErrorMessageResourceName = "StringLengthMin",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [DataType(DataType.Password)]
        [Display(Name = "RegisterPassword", ResourceType = typeof(DataAnnotations))]
        public required string Password { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [DataType(DataType.Password)]
        [Compare(
            nameof(Password),
            ErrorMessageResourceName = "PasswordMismatch",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "RegisterConfirmPassword", ResourceType = typeof(DataAnnotations))]
        public required string ConfirmPassword { get; set; }
    }
}
