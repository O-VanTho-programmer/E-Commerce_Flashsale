using ECommerce.Application.Common.Events;
using ECommerce.Application.Common.Interfaces.Services;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace ECommerce.Infrastructure.Messaging;

public class SendEmailOnOrderPlacedConsumer : IConsumer<OrderPlacedEvent>
{
    private readonly ILogger<SendEmailOnOrderPlacedConsumer> _logger;
    private readonly IEmailNotificationService _emailNotificationService;

    public SendEmailOnOrderPlacedConsumer(
        ILogger<SendEmailOnOrderPlacedConsumer> logger,
        IEmailNotificationService emailNotificationService)
    {
        _logger = logger;
        _emailNotificationService = emailNotificationService;
    }

    public async Task Consume(ConsumeContext<OrderPlacedEvent> context)
    {
        var message = context.Message;
        
        _logger.LogInformation("[Email Service] Receiving OrderPlacedEvent for OrderId: {OrderId}", message.OrderId);

        var recipient = $"user{message.UserId}@example.com";
        var subject = $"Order Confirmation - #{message.OrderId}";
        var body = $"Your order {message.OrderId} has been successfully placed.";

        await _emailNotificationService.SendAsync(recipient, subject, body);
    }
}

