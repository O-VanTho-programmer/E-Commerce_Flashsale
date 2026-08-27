using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IExternalOrderSyncLogRepository : IGenericRepository<ECommerce.Domain.Entities.ExternalOrderSyncLog>
{
    Task<ECommerce.Domain.Entities.ExternalOrderSyncLog?> GetByExternalOrderIdAsync(string platformName, string externalOrderId);
}
