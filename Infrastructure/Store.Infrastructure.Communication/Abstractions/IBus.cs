namespace Store.Infrastructure.Communication.Abstractions;

public interface IBus
{
    Task PublishAsync(Message message, CancellationToken token = default);
    Task SubscribeAsync<TAssemblyMarker>(CancellationToken token = default);
}