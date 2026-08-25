using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IStockReservationRepository : IGenericRepository<ECommerce.Domain.Entities.StockReservation>
{
    Task<int> GetActiveReservationsQuantityAsync(int productVariantId);
}
