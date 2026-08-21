namespace ECommerce.Domain.Entities;

public class CartItem : BaseEntity
{
    public int CartId { get; private set; }
    public int ProductVariantId { get; private set; }
    public int Quantity { get; private set; }
    public bool IsFlashSale { get; private set; }

    public Cart? Cart { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }
    public StockReservation? StockReservation { get; private set; }

    private CartItem() {}

    public CartItem(int cartId, int productVariantId, int quantity, bool isFlashSale = false)
    {
        CartId = cartId;
        ProductVariantId = productVariantId;
        Quantity = quantity;
        IsFlashSale = isFlashSale;
    }

    public void UpdateQuantity(int quantity)
    {
        Quantity = quantity;
    }
}
