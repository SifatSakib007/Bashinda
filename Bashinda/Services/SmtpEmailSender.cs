using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Logging;

namespace Bashinda.Services
{
    public class SmtpEmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SmtpEmailSender> _logger;

        public SmtpEmailSender(IConfiguration configuration, ILogger<SmtpEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            try
            {
                // Get SMTP settings with validation
                var smtpHost = _configuration["Smtp:Host"];
                if (string.IsNullOrEmpty(smtpHost))
                {
                    throw new InvalidOperationException("SMTP host is not configured");
                }

                var smtpPortStr = _configuration["Smtp:Port"];
                if (string.IsNullOrEmpty(smtpPortStr) || !int.TryParse(smtpPortStr, out int smtpPort))
                {
                    throw new InvalidOperationException("Invalid SMTP port configuration");
                }

                var smtpUser = _configuration["Smtp:User"];
                if (string.IsNullOrEmpty(smtpUser))
                {
                    throw new InvalidOperationException("SMTP username is not configured");
                }

                var smtpPass = _configuration["Smtp:Pass"];
                if (string.IsNullOrEmpty(smtpPass))
                {
                    throw new InvalidOperationException("SMTP password is not configured");
                }

                var fromEmail = _configuration["Smtp:FromEmail"];
                if (string.IsNullOrEmpty(fromEmail))
                {
                    throw new InvalidOperationException("SMTP from email is not configured");
                }

                var mail = new MailMessage();
                mail.From = new MailAddress(fromEmail);
                mail.To.Add(toEmail);
                mail.Subject = subject;
                mail.Body = message;

                using (var smtp = new SmtpClient(smtpHost, smtpPort))
                {
                    smtp.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    smtp.EnableSsl = true;
                    await smtp.SendMailAsync(mail);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending email to {ToEmail}", toEmail);
                throw;
            }
        }
    }
}
