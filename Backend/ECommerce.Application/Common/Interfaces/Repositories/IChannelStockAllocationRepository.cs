using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IChannelStockAllocationRepository : IGenericRepository<ECommerce.Domain.Entities.ChannelStockAllocation>
{
    Task<ECommerce.Domain.Entities.ChannelStockAllocation?> GetAllocationAsync(int productVariantId, string platformName);
}
