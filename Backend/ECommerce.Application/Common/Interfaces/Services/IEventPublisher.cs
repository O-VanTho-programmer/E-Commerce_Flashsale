using System.Threading;
using System.Threading.Tasks;

namespace ECommerce.Application.Common.Interfaces.Services;

public interface IEventPublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class;
}
