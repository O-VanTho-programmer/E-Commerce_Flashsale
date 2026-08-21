using System.Threading.Tasks;
using ECommerce.Application.Common.Interfaces.Repositories;
using ECommerce.Infrastructure.Data;

namespace ECommerce.Infrastructure.Repositories;

public class WebhookLogRepository : GenericRepository<ECommerce.Domain.Entities.WebhookLog>, IWebhookLogRepository
{
    public WebhookLogRepository(AppDbContext context) : base(context)
    {
    }
}
