using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class StockReservationRepository : GenericRepository<ECommerce.Domain.Entities.StockReservation>, IStockReservationRepository
{
    public StockReservationRepository(AppDbContext context) : base(context)
    {
    }
}
