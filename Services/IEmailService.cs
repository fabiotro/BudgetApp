namespace BudgetApp.Services
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string toEmail, string displayName, string confirmUrl);
        Task SendCampInviteEmailAsync(
            string toEmail,
            string displayName,
            string campName,
            string inviteUrl
        );
    }
}
