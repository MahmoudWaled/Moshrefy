using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moshrefy.Application.Interfaces.IServices;
using Moshrefy.Application.Settings;
using System.Net;
using System.Net.Mail;

namespace Moshrefy.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly EmailSettings _emailSettings;
        private readonly ILogger<EmailService> _logger;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _emailSettings = configuration.GetSection("EmailSettings").Get<EmailSettings>() ?? new EmailSettings();
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            try
            {
                // Validate email settings
                if (string.IsNullOrEmpty(_emailSettings.SmtpHost))
                {
                    _logger.LogError("SMTP Host is not configured");
                    throw new InvalidOperationException("Email service is not properly configured - SMTP Host is missing");
                }

                if (string.IsNullOrEmpty(_emailSettings.SenderEmail))
                {
                    _logger.LogError("Sender email is not configured");
                    throw new InvalidOperationException("Email service is not properly configured - Sender email is missing");
                }

                if (string.IsNullOrEmpty(_emailSettings.Username) || string.IsNullOrEmpty(_emailSettings.Password))
                {
                    _logger.LogError("Email credentials are not configured");
                    throw new InvalidOperationException("Email service is not properly configured - Credentials are missing");
                }

                _logger.LogInformation("Attempting to send email to {Email} via {SmtpHost}:{Port}", 
                    toEmail, _emailSettings.SmtpHost, _emailSettings.SmtpPort);

                using var smtpClient = new SmtpClient(_emailSettings.SmtpHost, _emailSettings.SmtpPort)
                {
                    EnableSsl = _emailSettings.EnableSsl,
                    Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
                    Timeout = 30000 // 30 seconds timeout
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    Subject = subject,
                    Body = htmlBody,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(toEmail);

                await smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Email sent successfully to {Email}", toEmail);
            }
            catch (SmtpException smtpEx)
            {
                _logger.LogError(smtpEx, "SMTP Error sending email to {Email}. Status: {Status}", 
                    toEmail, smtpEx.StatusCode);
                throw new InvalidOperationException($"Failed to send email: {smtpEx.Message}", smtpEx);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                throw;
            }
        }

        public async Task SendPasswordResetEmailAsync(string toEmail, string userName, string resetLink)
        {
            var subject = "Reset Your Password - Moshrefy";
            var htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #4CAF50; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f9f9f9; padding: 30px; }}
                        .button {{ display: inline-block; padding: 12px 30px; background-color: #4CAF50; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #777; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Password Reset Request</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {userName},</p>
                            <p>You have requested to reset your password. Click the button below to reset your password:</p>
                            <p style='text-align: center;'>
                                <a href='{resetLink}' class='button'>Reset Password</a>
                            </p>
                            <p>If you did not request a password reset, please ignore this email or contact support if you have concerns.</p>
                            <p>This link will expire in 24 hours.</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 Moshrefy. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendEmailConfirmationAsync(string toEmail, string userName, string confirmationLink)
        {
            var subject = "Confirm Your Email - Moshrefy";
            var htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #2196F3; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f9f9f9; padding: 30px; }}
                        .button {{ display: inline-block; padding: 12px 30px; background-color: #2196F3; color: white; text-decoration: none; border-radius: 5px; margin: 20px 0; }}
                        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #777; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Welcome to Moshrefy!</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {userName},</p>
                            <p>Thank you for registering with Moshrefy. Please confirm your email address by clicking the button below:</p>
                            <p style='text-align: center;'>
                                <a href='{confirmationLink}' class='button'>Confirm Email</a>
                            </p>
                            <p>If you did not create an account, please ignore this email.</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 Moshrefy. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendWelcomeEmailAsync(string toEmail, string userName)
        {
            var subject = "Welcome to Moshrefy!";
            var htmlBody = $@"
                <!DOCTYPE html>
                <html>
                <head>
                    <style>
                        body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                        .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                        .header {{ background-color: #FF9800; color: white; padding: 20px; text-align: center; }}
                        .content {{ background-color: #f9f9f9; padding: 30px; }}
                        .footer {{ text-align: center; margin-top: 20px; font-size: 12px; color: #777; }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <div class='header'>
                            <h1>Welcome to Moshrefy!</h1>
                        </div>
                        <div class='content'>
                            <p>Hello {userName},</p>
                            <p>Welcome to Moshrefy Educational Center Management System!</p>
                            <p>We're excited to have you on board. You can now manage your educational center efficiently with our comprehensive platform.</p>
                            <p>If you have any questions or need assistance, please don't hesitate to contact our support team.</p>
                            <p>Best regards,<br>The Moshrefy Team</p>
                        </div>
                        <div class='footer'>
                            <p>&copy; 2024 Moshrefy. All rights reserved.</p>
                        </div>
                    </div>
                </body>
                </html>";

            await SendEmailAsync(toEmail, subject, htmlBody);
        }
    }
}