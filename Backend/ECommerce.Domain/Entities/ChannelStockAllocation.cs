using System;

namespace ECommerce.Domain.Entities;

public class ChannelStockAllocation : BaseEntity
{
    public int ProductVariantId { get; private set; }
    public string PlatformName { get; private set; }
    public int AllocatedQuantity { get; private set; }
    public int SoldQuantity { get; private set; }
    public byte[]? RowVersion { get; private set; }

    public ProductVariant? ProductVariant { get; private set; }

    private ChannelStockAllocation() 
    {
        PlatformName = string.Empty;
    }

    public ChannelStockAllocation(int productVariantId, string platformName, int allocatedQuantity)
    {
        ProductVariantId = productVariantId;
        PlatformName = platformName;
        AllocatedQuantity = allocatedQuantity;
        SoldQuantity = 0;
    }

    public void AllocateMore(int quantity)
    {
        AllocatedQuantity += quantity;
    }

    public void RecordSale(int quantity)
    {
        SoldQuantity += quantity;
        
        // Note: We intentionally allow SoldQuantity to exceed AllocatedQuantity
        // because in the real world, if Shopee sold it, they already took the money.
        // It's better to log a negative available balance than to drop the webhook.
    }

    public int GetAvailableAllocation()
    {
        return AllocatedQuantity - SoldQuantity;
    }
}
