using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Models.Configurations;

namespace ASPNET_DK24TTC6_LuuThiNgoc_CafeShop.Services;

public class EmailService : IEmailService
{
    private readonly EmailOptions _options;
    private readonly ILogger<EmailService> _logger;

    public EmailService(IOptions<EmailOptions> options, ILogger<EmailService> logger)
    {
        _options = options.Value;
        _logger = logger;
    }

    public async Task SendAsync(string toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrWhiteSpace(_options.SmtpHost) ||
            string.IsNullOrWhiteSpace(_options.FromEmail))
        {
            _logger.LogWarning(
                "Email configuration missing. Subject={Subject}, To={ToEmail}. Content logged for dev fallback.",
                subject,
                toEmail);
            _logger.LogInformation("Email preview: {Body}", htmlBody);
            return;
        }

        using var mail = new MailMessage
        {
            From = new MailAddress(_options.FromEmail, _options.FromName),
            Subject = subject,
            Body = htmlBody,
            IsBodyHtml = true
        };
        mail.To.Add(toEmail);

        using var smtp = new SmtpClient(_options.SmtpHost, _options.SmtpPort)
        {
            EnableSsl = _options.EnableSsl
        };

        if (!string.IsNullOrWhiteSpace(_options.Username))
        {
            smtp.Credentials = new NetworkCredential(_options.Username, _options.Password);
        }

        await smtp.SendMailAsync(mail);
    }
}
