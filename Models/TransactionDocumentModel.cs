using System.ComponentModel.DataAnnotations;

namespace BudgetApp.Models
{
    public class TransactionDocumentModel
    {
        public int Id { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int TransactionId { get; set; }

        [Required]
        [StringLength(255)]
        public required string FileName { get; set; }

        [Required]
        [StringLength(100)]
        public required string ContentType { get; set; }

        public int FileSize { get; set; }

        // Populated only by GetById (download); null when listing metadata
        public byte[]? FileData { get; set; }
    }
}
