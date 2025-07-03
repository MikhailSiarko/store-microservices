namespace Store.Infrastructure.Communication.Abstractions;

public abstract class MessageBase
{
    public DateTime Timestamp { get; protected set; } = DateTime.UtcNow;
}