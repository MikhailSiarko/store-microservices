namespace Store.Infrastructure.Communication.Abstractions;

public interface IEventHandler<in TEvent> : IEventHandler where TEvent : Event
{
    Task HandleAsync(TEvent @event, CancellationToken token = default);
}

public interface IEventHandler;