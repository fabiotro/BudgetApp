using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class CampUserRoleModel
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }
    }
}
