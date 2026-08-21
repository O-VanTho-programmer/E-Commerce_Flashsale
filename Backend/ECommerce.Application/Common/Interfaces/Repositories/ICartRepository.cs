using System.Threading.Tasks;
using ECommerce.Domain.Entities;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface ICartRepository : IGenericRepository<ECommerce.Domain.Entities.Cart>
{
    Task<ECommerce.Domain.Entities.Cart?> GetByUserIdWithItemsAsync(int userId);
}
