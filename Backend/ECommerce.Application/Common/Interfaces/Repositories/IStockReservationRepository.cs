using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IStockReservationRepository : IGenericRepository<ECommerce.Domain.Entities.StockReservation>
{
    Task<int> GetActiveReservationsQuantityAsync(int productVariantId);
    Task<System.Collections.Generic.IEnumerable<ECommerce.Domain.Entities.StockReservation>> GetConfirmedReservationsByOrderIdAsync(int orderId);
}
