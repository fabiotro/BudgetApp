using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class UserModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public required string Email { get; set; }

        [Required]
        [StringLength(100)]
        public required string DisplayName { get; set; }

        [Required]
        [StringLength(500)]
        public required string PasswordHash { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }

        [StringLength(34)]
        public string? IBAN { get; set; }
    }
}
