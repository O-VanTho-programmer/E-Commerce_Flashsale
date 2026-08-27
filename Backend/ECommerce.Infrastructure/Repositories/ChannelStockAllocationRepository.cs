using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ChannelStockAllocationRepository : GenericRepository<ECommerce.Domain.Entities.ChannelStockAllocation>, IChannelStockAllocationRepository
{
    public ChannelStockAllocationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ECommerce.Domain.Entities.ChannelStockAllocation?> GetAllocationAsync(int productVariantId, string platformName)
    {
        return await _dbSet.FirstOrDefaultAsync(c => c.ProductVariantId == productVariantId && c.PlatformName == platformName);
    }
}
