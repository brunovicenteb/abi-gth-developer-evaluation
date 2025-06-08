namespace Ambev.DeveloperEvaluation.Domain.Common.Messaging;

public interface IMessageBus
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default);
}