using Microsoft.AspNetCore.Identity.UI.Services;
using System.Net;
using System.Net.Mail;

namespace CarRental.Web.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            // Google SMTP Configuration
            // Requires 'Less Secure Apps' enabled or App Password (recommended)
            var emailFrom = _configuration["Authentication:Google:Email"];
            var password = _configuration["Authentication:Google:AppPassword"];
            
            if (string.IsNullOrEmpty(emailFrom) || string.IsNullOrEmpty(password))
            {
                Console.WriteLine($"[EmailSender] Config missing. Mock sending email to {email}: {subject}");
                return;
            }

            try
            {
                var client = new SmtpClient("smtp.gmail.com", 587)
                {
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(emailFrom, password)
                };

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(emailFrom, "Aurum Veloce Support"),
                    Subject = subject,
                    Body = htmlMessage,
                    IsBodyHtml = true
                };
                mailMessage.To.Add(email);

                await client.SendMailAsync(mailMessage);
                Console.WriteLine($"[EmailSender] Sent email to {email}");
            }
            catch (Exception ex)
            {
                 Console.WriteLine($"[EmailSender] Error sending email: {ex.Message}");
                 throw;
            }
        }
    }
}
