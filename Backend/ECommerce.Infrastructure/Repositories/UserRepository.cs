using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class UserRepository : GenericRepository<ECommerce.Domain.Entities.User>, IUserRepository
{
    public UserRepository(AppDbContext context) : base(context)
    {
    }
}
