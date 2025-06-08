using Rebus.Bus;
using Ambev.DeveloperEvaluation.Domain.Common.Messaging;

namespace Ambev.DeveloperEvaluation.ORM.Common.Messaging;

public class RebusMessageBus : IMessageBus
{
    private readonly IBus _bus;

    public RebusMessageBus(IBus bus)
    {
        _bus = bus;
    }

    public Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
    {
        return _bus.Publish(message);
    }
}