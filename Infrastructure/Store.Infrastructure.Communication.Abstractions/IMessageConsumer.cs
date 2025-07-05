namespace Store.Infrastructure.Communication.Abstractions;

public interface IMessageConsumer<in TMessage> where TMessage : Message
{
    Task HandleAsync(TMessage message, CancellationToken token = default);
}