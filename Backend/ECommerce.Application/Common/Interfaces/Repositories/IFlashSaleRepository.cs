using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IFlashSaleRepository : IGenericRepository<ECommerce.Domain.Entities.FlashSale>
{
    Task<ECommerce.Domain.Entities.FlashSale?> GetActiveFlashSaleWithItemsAsync();
}
