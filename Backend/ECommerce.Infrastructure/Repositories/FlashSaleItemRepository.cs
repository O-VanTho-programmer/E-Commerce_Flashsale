using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class FlashSaleItemRepository : GenericRepository<ECommerce.Domain.Entities.FlashSaleItem>, IFlashSaleItemRepository
{
    public FlashSaleItemRepository(AppDbContext context) : base(context)
    {
    }
}
