namespace ECommerce.Domain.Entities;

public class FlashSaleItem : BaseEntity
{
    public int FlashSaleId { get; set; }
    public int ProductVariantId { get; set; }
    public decimal SalePrice { get; set; }
    public int SaleStock { get; set; }
    public int SoldCount { get; set; }
    public byte[]? RowVersion { get; set; }

    public FlashSale? FlashSale { get; set; }
    public ProductVariant? ProductVariant { get; set; }
}
