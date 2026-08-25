using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Services;

public interface IInventoryService
{
    Task<bool> IsStockAvailableAsync(int productVariantId, int requestedQuantity, bool isFlashSale);
}
