namespace Store.Infrastructure.Communication.Abstractions;

public interface ICommunicationBus
{
    Task PublishAsync(Message message, CancellationToken token = default);
    Task SubscribeAsync<TAssemblyMarker>(string subscriptionName, CancellationToken token = default);
}