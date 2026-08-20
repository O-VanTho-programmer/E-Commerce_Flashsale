using System;
using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class Cart : BaseEntity
{
    public int UserId { get; set; }
    public DateTime UpdatedAt { get; set; }

    public User? User { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}
