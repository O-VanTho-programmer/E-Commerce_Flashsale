using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; private set; }
    public string OrderCode { get; private set; }
    public Enums.OrderStatus Status { get; private set; }
    public decimal TotalAmount { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public byte[]? RowVersion { get; private set; }

    public User? User { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();
    public Payment? Payment { get; private set; }

    private Order()
    {
        OrderCode = string.Empty;
    }

    public Order(int userId, string orderCode, decimal totalAmount)
    {
        UserId = userId;
        OrderCode = orderCode;
        TotalAmount = totalAmount;
        Status = Enums.OrderStatus.Pending;
        CreatedAt = DateTime.UtcNow;
    }

    public void TransitionTo(Enums.OrderStatus newStatus)
    {
        // Validation could be added here based on _allowedTransitions rule
        Status = newStatus;
    }
}
