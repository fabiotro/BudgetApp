namespace BudgetApp.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string toEmail, string displayName, string confirmUrl);
        Task SendBudgetInviteEmailAsync(
            string toEmail,
            string displayName,
            string budgetName,
            string inviteUrl
        );
    }
}
