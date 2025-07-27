// Infrastructure/Services/EmailService.cs
using Microsoft.Extensions.Configuration;
using Services.Abstractions;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

public class EmailService : IEmailService
{
    private readonly IConfiguration _config;

    public EmailService(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendOrderStatusChangedEmailAsync(string toEmail, int orderId, string newStatus)
    {
        var smtpClient = new SmtpClient(_config["Email:SmtpHost"])
        {
            Port = int.Parse(_config["Email:SmtpPort"]),
            Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
            EnableSsl = true,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_config["Email:From"]),
            Subject = "Order Status Updated",
            Body = $"Your order #{orderId} status has been updated to: {newStatus}.",
            IsBodyHtml = false,
        };

        mailMessage.To.Add(toEmail);
        await smtpClient.SendMailAsync(mailMessage);
    }
}
