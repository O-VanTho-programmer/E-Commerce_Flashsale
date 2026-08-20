using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; set; }
    public string Provider { get; set; } = string.Empty;
    public Enums.PaymentStatus Status { get; set; }
    public decimal Amount { get; set; }
    public DateTime PaidAt { get; set; }

    public Order? Order { get; set; }
    public ICollection<WebhookLog> WebhookLogs { get; set; } = new List<WebhookLog>();
}
