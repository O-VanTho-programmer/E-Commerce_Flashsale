using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace ECommerce.Infrastructure.Repositories;

public class ExternalOrderSyncLogRepository : GenericRepository<ECommerce.Domain.Entities.ExternalOrderSyncLog>, IExternalOrderSyncLogRepository
{
    public ExternalOrderSyncLogRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<ECommerce.Domain.Entities.ExternalOrderSyncLog?> GetByExternalOrderIdAsync(string platformName, string externalOrderId)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.PlatformName == platformName && e.ExternalOrderId == externalOrderId);
    }
}
