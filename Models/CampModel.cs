using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class CampModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public int CreatedByUserId { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int ParticipantsCount_fc { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int js_PersonsCount_fc { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        public int LeadersTeamCount_fc { get; set; }

        [Range(0, int.MaxValue)]
        public int? ParticipantsCount_rl { get; set; }

        [Range(0, int.MaxValue)]
        public int? js_PersonsCount_rl { get; set; }

        [Range(0, int.MaxValue)]
        public int? LeadersTeamCount_rl { get; set; }
    }
}
