using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Application.Common.Interfaces.Services;

namespace ECommerce.Infrastructure.Services;

public class InventoryService : IInventoryService
{
    private readonly IUnitOfWork _unitOfWork;

    public InventoryService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> IsStockAvailableAsync(int productVariantId, int requestedQuantity, bool isFlashSale)
    {
        int activeReservations = await _unitOfWork.StockReservations.GetActiveReservationsQuantityAsync(productVariantId);

        //Check Flash Sale Stock
        if (isFlashSale)
        {
            var flashSaleItem = await _unitOfWork.FlashSaleItems.FirstOrDefaultAsync(f => f.ProductVariantId == productVariantId);
            if (flashSaleItem == null) return false;

            // Available = SaleStock - Already Checked Out (SoldCount) - Currently Reserved by others
            int availableStock = flashSaleItem.SaleStock - flashSaleItem.SoldCount - activeReservations;
            return availableStock >= requestedQuantity;
        }

        // 3. Check Regular Stock
        var variant = await _unitOfWork.ProductVariants.GetByIdAsync(productVariantId);
        if (variant == null) return false;

        int availableRegularStock = variant.StockQuantity - activeReservations;
        return availableRegularStock >= requestedQuantity;
    }
}
