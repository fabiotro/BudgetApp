using System.ComponentModel.DataAnnotations;
using BudgetApp.Enums;
using BudgetApp.Resources;

namespace BudgetApp.Models
{
    public class CreateTransactionViewModel
    {
        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Range(1, int.MaxValue)]
        public int BudgetId { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Range(
            1,
            int.MaxValue,
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "TransactionPerformedBy", ResourceType = typeof(DataAnnotations))]
        public int PerformedByUserId { get; set; }

        public int? PositionId { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [StringLength(
            255,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "TransactionName", ResourceType = typeof(DataAnnotations))]
        public required string Name { get; set; }

        [StringLength(
            500,
            ErrorMessageResourceName = "StringLength",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "TransactionDescription", ResourceType = typeof(DataAnnotations))]
        public string? Description { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Range(
            0.01,
            double.MaxValue,
            ErrorMessageResourceName = "Range",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "TransactionAmount", ResourceType = typeof(DataAnnotations))]
        public decimal Amount { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "TransactionPaymentSource", ResourceType = typeof(DataAnnotations))]
        public PaymentSource PaymentSource { get; set; }

        [Required(
            ErrorMessageResourceName = "Required",
            ErrorMessageResourceType = typeof(DataAnnotations)
        )]
        [Display(Name = "TransactionPaymentMethod", ResourceType = typeof(DataAnnotations))]
        public PaymentMethod PaymentMethod { get; set; }

        // Not posted — populated by controller for select lists
        public List<BudgetUserModel> BudgetUsers { get; set; } = [];
        public List<PositionModel> Positions { get; set; } = [];
    }

    public class EditTransactionViewModel : CreateTransactionViewModel
    {
        public int Id { get; set; }
        public List<TransactionDocumentModel> ExistingDocuments { get; set; } = [];
    }

    public class TransactionWithDocumentsViewModel
    {
        public required TransactionModel Transaction { get; set; }
        public List<TransactionDocumentModel> Documents { get; set; } = [];
        public string PerformedByDisplayName { get; set; } = string.Empty;
        public string? PositionName { get; set; }
    }

    public class UserTransactionsViewModel
    {
        public required BudgetModel Budget { get; set; }
        public required UserExpenseSummaryViewModel UserSummary { get; set; }
        public List<TransactionWithDocumentsViewModel> Transactions { get; set; } = [];
    }
}
