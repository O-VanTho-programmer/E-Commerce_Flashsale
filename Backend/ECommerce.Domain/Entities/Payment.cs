using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Payment : BaseEntity
{
    public int OrderId { get; private set; }
    public string Provider { get; private set; }
    public Enums.PaymentStatus Status { get; private set; }
    public decimal Amount { get; private set; }
    public DateTime PaidAt { get; private set; }

    public Order? Order { get; private set; }
    public ICollection<WebhookLog> WebhookLogs { get; private set; } = new List<WebhookLog>();

    private Payment()
    {
        Provider = string.Empty;
    }

    public Payment(int orderId, string provider, decimal amount)
    {
        OrderId = orderId;
        Provider = provider;
        Amount = amount;
        Status = Enums.PaymentStatus.Pending;
        PaidAt = DateTime.UtcNow;
    }

    public void UpdateStatus(Enums.PaymentStatus status)
    {
        Status = status;
    }
}
