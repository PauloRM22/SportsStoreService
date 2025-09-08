using MassTransit;
using StoreService.Application.Messaging;

namespace StoreService.Infrastructure.Messaging;

public class MassTransitEventPublisher : IEventPublisher
{
    private readonly IPublishEndpoint _endpoint;
    public MassTransitEventPublisher(IPublishEndpoint endpoint) => _endpoint = endpoint;
    public Task PublishAsync<T>(T message, CancellationToken ct = default) where T : class
        => _endpoint.Publish(message, ct);
}