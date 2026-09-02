using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Services;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Services;

public class EmailNotificationService : IEmailNotificationService
{
    private readonly ILogger<EmailNotificationService> _logger;

    public EmailNotificationService(ILogger<EmailNotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendAsync(string recipient, string subject, string message)
    {
        _logger.LogInformation("[EmailProvider] Preparing to send email to {Recipient}", recipient);
        
        // Simulate sending email (calling external SMTP service like SendGrid, SES)
        await Task.Delay(500); 
        
        _logger.LogInformation("[EmailProvider] Successfully sent email to {Recipient}. Subject: {Subject}", recipient, subject);
    }
}
