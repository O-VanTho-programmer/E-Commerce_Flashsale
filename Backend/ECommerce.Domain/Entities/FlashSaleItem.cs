namespace ECommerce.Domain.Entities;

public class FlashSaleItem : BaseEntity
{
    public int FlashSaleId { get; private set; }
    public int ProductVariantId { get; private set; }
    public decimal SalePrice { get; private set; }
    public int SaleStock { get; private set; }
    public int SoldCount { get; private set; }
    public byte[]? RowVersion { get; private set; }

    public FlashSale? FlashSale { get; private set; }
    public ProductVariant? ProductVariant { get; private set; }

    private FlashSaleItem() {}

    public FlashSaleItem(int flashSaleId, int productVariantId, decimal salePrice, int saleStock)
    {
        FlashSaleId = flashSaleId;
        ProductVariantId = productVariantId;
        SalePrice = salePrice;
        SaleStock = saleStock;
        SoldCount = 0;
    }

    public void IncrementSoldCount(int quantity)
    {
        if (SoldCount + quantity > SaleStock)
            throw new Exception("Sale stock exceeded.");
        SoldCount += quantity;
    }
}
