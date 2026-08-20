using System.Collections.Generic;

namespace ECommerce.Domain.Entities;

public class ProductVariant : BaseEntity
{
    public int ProductId { get; set; }
    public string Sku { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string Size { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int StockQuantity { get; set; }
    public byte[]? RowVersion { get; set; }

    public Product? Product { get; set; }
    public FlashSaleItem? FlashSaleItem { get; set; }
    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
