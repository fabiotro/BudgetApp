using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class CampUserModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CampId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int UserId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int CampUserRoleId { get; set; }

        // Joined from User table for display
        public string? DisplayName { get; set; }
        public string? Email { get; set; }

        // Joined from CampUserRole table for display
        public string? RoleName { get; set; }
    }
}
