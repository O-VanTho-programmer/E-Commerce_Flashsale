using ECommerce.Application.Common.Interfaces.Services;
using MassTransit;

namespace ECommerce.Infrastructure.Messaging
{
    public class EventPublisher : IEventPublisher
    {
        private readonly IPublishEndpoint _publisherEndpoint;

        public EventPublisher(IPublishEndpoint publisherEndpoint)
        {
            _publisherEndpoint = publisherEndpoint;
        }

        public async Task PublishAsync<T>(T message, CancellationToken cancellationToken = default) where T : class
        {
            await _publisherEndpoint.Publish(message, cancellationToken);
        }
    }
}
