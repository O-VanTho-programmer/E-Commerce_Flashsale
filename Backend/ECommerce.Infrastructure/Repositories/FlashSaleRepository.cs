using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class FlashSaleRepository : GenericRepository<ECommerce.Domain.Entities.FlashSale>, IFlashSaleRepository
{
    public FlashSaleRepository(AppDbContext context) : base(context)
    {
    }
}
