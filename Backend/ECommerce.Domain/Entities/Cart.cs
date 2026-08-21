using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Cart : BaseEntity
{
    public int UserId { get; private set; }
    public DateTime UpdatedAt { get; private set; }

    public User? User { get; private set; }
    public ICollection<CartItem> CartItems { get; private set; } = new List<CartItem>();

    private Cart() {}

    public Cart(int userId)
    {
        UserId = userId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}
