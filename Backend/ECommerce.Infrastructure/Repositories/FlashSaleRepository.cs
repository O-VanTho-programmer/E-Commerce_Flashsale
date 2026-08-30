using System.Threading.Tasks;
using System.Linq;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class FlashSaleRepository : GenericRepository<ECommerce.Domain.Entities.FlashSale>, IFlashSaleRepository
{
    public FlashSaleRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ECommerce.Domain.Entities.FlashSale?> GetActiveFlashSaleWithItemsAsync()
    {
        var now = System.DateTime.UtcNow;
        return await _dbSet
            .Include(f => f.FlashSaleItems)
                .ThenInclude(fi => fi.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .Where(f => f.StartAt <= now && f.EndAt >= now && f.Status == Domain.Enums.FlashSaleStatus.Active)
            .FirstOrDefaultAsync();
    }
}
