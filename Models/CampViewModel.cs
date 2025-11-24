using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class CampViewModel
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Lagerbeginn")]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Lagerende")]
        public DateTime EndDate { get; set; }

        [StringLength(255)]
        [Display(Name = "Hauptleitung")]
        public string? MainLeader { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Anzahl Teilnehmende (TN)")]
        public int ParticipantsCount_fc { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "J+S-angemeldete Personen")]
        public int js_PersonsCount_fc { get; set; }

        [Required]
        [Range(0, int.MaxValue)]
        [Display(Name = "Leitung, Küche, Hilfspersonen")]
        public int LeadersTeamCount_fc { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Anzahl Teilnehmende (TN)")]
        public int? ParticipantsCount_rl { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "J+S-angemeldete Personen")]
        public int? js_PersonsCount_rl { get; set; }

        [Range(0, int.MaxValue)]
        [Display(Name = "Leitung, Küche, Hilfspersonen")]
        public int? LeadersTeamCount_rl { get; set; }
    }
}
