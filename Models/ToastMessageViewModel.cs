using BudgetApp.Enums;

namespace BudgetApp.Models
{
    public class ToastMessageViewModel
    {
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public ToastType Type { get; set; } = ToastType.Info;
    }
}
