using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class StockReservationRepository : GenericRepository<ECommerce.Domain.Entities.StockReservation>, IStockReservationRepository
{
    public StockReservationRepository(AppDbContext context) : base(context)
    {
    }

    public async Task<int> GetActiveReservationsQuantityAsync(int productVariantId)
    {
        var now = System.DateTime.UtcNow;
        return await _dbSet
            .Where(r => r.ProductVariantId == productVariantId 
                        && (
                            (r.Status == Domain.Enums.StockReservationStatus.Reserved && r.ExpiresAt > now)
                            || r.Status == Domain.Enums.StockReservationStatus.Confirmed
                           )
                  )
            .SumAsync(r => r.Quantity);
    }

    public async Task<System.Collections.Generic.IEnumerable<ECommerce.Domain.Entities.StockReservation>> GetConfirmedReservationsByOrderIdAsync(int orderId)
    {
        return await _dbSet
            .Where(r => r.OrderId == orderId && r.Status == Domain.Enums.StockReservationStatus.Confirmed)
            .ToListAsync();
    }
}
