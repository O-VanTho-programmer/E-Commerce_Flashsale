using System;

namespace ECommerce.Domain.Entities;

public class WebhookLog : BaseEntity
{
    public int PaymentId { get; set; }
    public string WebhookEventId { get; set; } = string.Empty; // UK
    public string Payload { get; set; } = string.Empty;
    public Enums.WebhookProcessStatus ProcessStatus { get; set; }
    public DateTime ReceivedAt { get; set; }

    public Payment? Payment { get; set; }
}
