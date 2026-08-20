using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Order : BaseEntity
{
    public int UserId { get; set; }
    public string OrderCode { get; set; } = string.Empty;
    public Enums.OrderStatus Status { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public byte[]? RowVersion { get; set; }

    public User? User { get; set; }
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    public Payment? Payment { get; set; }
}
