using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Repositories;

public interface IAuditLogRepository : IGenericRepository<ECommerce.Domain.Entities.AuditLog>
{
}
