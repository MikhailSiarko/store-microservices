namespace Store.Infrastructure.Communication.Abstractions;

public interface ICommunicationBus
{
    Task PublishAsync(Message message, CancellationToken token = default);
    Task SubscribeAsync<TMessage, TMessageConsumer>(CancellationToken token = default)
        where TMessage : Message
        where TMessageConsumer : IMessageConsumer<TMessage>;
}