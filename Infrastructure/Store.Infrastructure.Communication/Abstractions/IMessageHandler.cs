namespace Store.Infrastructure.Communication.Abstractions;

public interface IMessageHandler<in TMessage> where TMessage : Message
{
    Task HandleAsync(TMessage message, CancellationToken token = default);
}