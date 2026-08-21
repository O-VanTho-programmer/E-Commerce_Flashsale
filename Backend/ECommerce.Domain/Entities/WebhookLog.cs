using System;

namespace ECommerce.Domain.Entities;

public class WebhookLog : BaseEntity
{
    public int PaymentId { get; private set; }
    public string WebhookEventId { get; private set; } // UK
    public string Payload { get; private set; }
    public Enums.WebhookProcessStatus ProcessStatus { get; private set; }
    public DateTime ReceivedAt { get; private set; }

    public Payment? Payment { get; private set; }

    private WebhookLog()
    {
        WebhookEventId = string.Empty;
        Payload = string.Empty;
    }

    public WebhookLog(int paymentId, string webhookEventId, string payload)
    {
        PaymentId = paymentId;
        WebhookEventId = webhookEventId;
        Payload = payload;
        ProcessStatus = Enums.WebhookProcessStatus.Pending;
        ReceivedAt = DateTime.UtcNow;
    }

    public void UpdateProcessStatus(Enums.WebhookProcessStatus status)
    {
        ProcessStatus = status;
    }
}
