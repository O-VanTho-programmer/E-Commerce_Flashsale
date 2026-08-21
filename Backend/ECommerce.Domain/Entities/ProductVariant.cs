using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; private set; }
    public string Sku { get; private set; }
    public string Color { get; private set; }
    public string Size { get; private set; }
    public decimal Price { get; private set; }
    public int StockQuantity { get; private set; }
    public byte[]? RowVersion { get; private set; }

    public Product? Product { get; private set; }
    public FlashSaleItem? FlashSaleItem { get; private set; }
    public ICollection<CartItem> CartItems { get; private set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; private set; } = new List<OrderItem>();

    private ProductVariant()
    {
        Sku = string.Empty;
        Color = string.Empty;
        Size = string.Empty;
    }

    public ProductVariant(int productId, string sku, string color, string size, decimal price, int stockQuantity)
    {
        ProductId = productId;
        Sku = sku;
        Color = color;
        Size = size;
        Price = price;
        StockQuantity = stockQuantity;
    }

    public void UpdateStock(int quantityDelta)
    {
        if (StockQuantity + quantityDelta < 0)
            throw new System.Exception("Insufficient stock.");
        StockQuantity += quantityDelta;
    }
}
