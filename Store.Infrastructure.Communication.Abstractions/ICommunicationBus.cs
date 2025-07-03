namespace Store.Infrastructure.Communication.Abstractions;

public interface ICommunicationBus
{
    Task SendAsync(Command command, CancellationToken token = default);
    Task PublishAsync(Event @event, CancellationToken token = default);
    Task SubscribeAsync<TEvent, TEventHandler>(CancellationToken token = default)
        where TEvent : Event
        where TEventHandler : IEventHandler<TEvent>;
}