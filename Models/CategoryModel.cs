using System.ComponentModel.DataAnnotations;
using BudgetApp.Resources;

namespace BudgetApp.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [StringLength(
            100,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "CategoryName", ResourceType = typeof(DataAnnotations))]
        public required string Name { get; set; }

        [StringLength(
            255,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "CategoryDescription", ResourceType = typeof(DataAnnotations))]
        public string? Description { get; set; }

        public int SortIndex { get; set; }
    }
}
