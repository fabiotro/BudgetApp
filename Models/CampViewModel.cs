using BudgetApp.Resources;
using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class CampViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [DataType(DataType.Date)]
        [Display(Name = "CampStartDate", ResourceType = typeof(DataAnnotations))]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [DataType(DataType.Date)]
        [Display(Name = "CampEndDate", ResourceType = typeof(DataAnnotations))]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "CampParticipantsCount", ResourceType = typeof(DataAnnotations))]
        public int? ParticipantsCount_fc { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "CampJsPersonsCount", ResourceType = typeof(DataAnnotations))]
        public int? js_PersonsCount_fc { get; set; }

        [Required(ErrorMessageResourceName = "Required", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Range(0, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "CampLeadersTeamCount", ResourceType = typeof(DataAnnotations))]
        public int? LeadersTeamCount_fc { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "CampParticipantsCount", ResourceType = typeof(DataAnnotations))]
        public int? ParticipantsCount_rl { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "CampJsPersonsCount", ResourceType = typeof(DataAnnotations))]
        public int? js_PersonsCount_rl { get; set; }

        [Range(0, int.MaxValue, ErrorMessageResourceName = "Range", ErrorMessageResourceType = typeof(DataAnnotations))]
        [Display(Name = "CampLeadersTeamCount", ResourceType = typeof(DataAnnotations))]
        public int? LeadersTeamCount_rl { get; set; }
    }
}
