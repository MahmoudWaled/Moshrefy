namespace Moshrefy.Application.Interfaces.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string toEmail, string subject, string htmlBody);
        Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink);
        Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmationLink);
        Task SendWelcomeEmailAsync(string toEmail, string userName);
    }
}