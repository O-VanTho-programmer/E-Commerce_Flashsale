using System;

namespace ECommerce.Domain.Entities;

public class StockReservation : BaseEntity
{
    public int CartItemId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public DateTime ExpiresAt { get; set; }
    public Enums.StockReservationStatus Status { get; set; }

    public CartItem? CartItem { get; set; }
}
