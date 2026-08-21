using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class ProductRepository : GenericRepository<ECommerce.Domain.Entities.Product>, IProductRepository
{
    public ProductRepository(AppDbContext context) : base(context)
    {
    }
}
