using System.ComponentModel.DataAnnotations;
using BudgetApp.Resources;

namespace BudgetApp.Models
{
    // Single-step create/edit form for a Budget's own data (dates, leader,
    // participant counts, name, description). Template selection is only
    // meaningful on create — SelectedTemplateBudgetId is ignored on edit.
    public class BudgetFormViewModel
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
        [Display(Name = "TemplateBudgetName", ResourceType = typeof(DataAnnotations))]
        public string Name { get; set; } = string.Empty;

        [StringLength(
            255,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "TemplateBudgetDescription", ResourceType = typeof(DataAnnotations))]
        public string? Description { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [DataType(DataType.Date)]
        [Display(Name = "CampStartDate", ResourceType = typeof(DataAnnotations))]
        public DateTime StartDate { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [DataType(DataType.Date)]
        [Display(Name = "CampEndDate", ResourceType = typeof(DataAnnotations))]
        public DateTime EndDate { get; set; }

        [StringLength(
            255,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "CampMainLeader", ResourceType = typeof(DataAnnotations))]
        public string? MainLeader { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Range(
            0,
            int.MaxValue,
            ErrorMessageResourceName = "Range",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "CampParticipantsCount", ResourceType = typeof(DataAnnotations))]
        public int? ParticipantsCount_fc { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Range(
            0,
            int.MaxValue,
            ErrorMessageResourceName = "Range",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "CampJsPersonsCount", ResourceType = typeof(DataAnnotations))]
        public int? js_PersonsCount_fc { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Range(
            0,
            int.MaxValue,
            ErrorMessageResourceName = "Range",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "CampLeadersTeamCount", ResourceType = typeof(DataAnnotations))]
        public int? LeadersTeamCount_fc { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessageResourceName = "Range",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "CampParticipantsCount", ResourceType = typeof(DataAnnotations))]
        public int? ParticipantsCount_rl { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessageResourceName = "Range",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "CampJsPersonsCount", ResourceType = typeof(DataAnnotations))]
        public int? js_PersonsCount_rl { get; set; }

        [Range(
            0,
            int.MaxValue,
            ErrorMessageResourceName = "Range",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "CampLeadersTeamCount", ResourceType = typeof(DataAnnotations))]
        public int? LeadersTeamCount_rl { get; set; }

        public int? SelectedTemplateBudgetId { get; set; }

        public List<TemplateBudgetModel> AvailableTemplates { get; set; } = [];
    }
}
