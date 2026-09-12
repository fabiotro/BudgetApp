using System.ComponentModel.DataAnnotations;
using BudgetApp.Resources;

namespace BudgetApp.Models
{
    public class EditProfileViewModel
    {
        public string Email { get; set; } = string.Empty;

        public bool IsEmailConfirmed { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [StringLength(
            100,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "ProfileDisplayName", ResourceType = typeof(DataAnnotations))]
        public required string DisplayName { get; set; }

        [StringLength(
            100,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "ProfileFirstName", ResourceType = typeof(DataAnnotations))]
        public string? FirstName { get; set; }

        [StringLength(
            100,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "ProfileLastName", ResourceType = typeof(DataAnnotations))]
        public string? LastName { get; set; }

        [StringLength(
            34,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "ProfileIBAN", ResourceType = typeof(DataAnnotations))]
        public string? IBAN { get; set; }
    }
}
