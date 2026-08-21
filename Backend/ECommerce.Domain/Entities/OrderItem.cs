namespace ECommerce.Domain.Entities;

public class OrderItem : BaseEntity
{
    public int OrderId { get; private set; }
    public int ProductVariantId { get; private set; }
    public int Quantity { get; private set; }
    public decimal UnitPrice { get; private set; }

    public Order? Order { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }

    private OrderItem() {}

    public OrderItem(int orderId, int productVariantId, int quantity, decimal unitPrice)
    {
        OrderId = orderId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }
}
