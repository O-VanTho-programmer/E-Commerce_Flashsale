using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IOrderRepository : IGenericRepository<ECommerce.Domain.Entities.Order>
{
    Task<ECommerce.Domain.Entities.Order?> GetByIdWithDetailsAsync(int orderId);
}
