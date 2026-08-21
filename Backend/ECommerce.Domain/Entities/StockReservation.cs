using System;

namespace ECommerce.Domain.Entities;

public class StockReservation : BaseEntity
{
    public int CartItemId { get; private set; }
    public int ProductVariantId { get; private set; }
    public int Quantity { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public Enums.StockReservationStatus Status { get; private set; }

    public CartItem? CartItem { get; private set; }

    private StockReservation() {}

    public StockReservation(int cartItemId, int productVariantId, int quantity, DateTime expiresAt)
    {
        CartItemId = cartItemId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        ExpiresAt = expiresAt;
        Status = Enums.StockReservationStatus.Reserved;
    }

    public void UpdateStatus(Enums.StockReservationStatus status)
    {
        Status = status;
    }
}
