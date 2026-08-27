using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IUserRepository : IGenericRepository<ECommerce.Domain.Entities.User>
{
    Task<ECommerce.Domain.Entities.User?> GetByEmailAsync(string email);
}
