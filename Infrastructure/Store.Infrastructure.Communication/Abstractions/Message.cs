namespace Store.Infrastructure.Communication.Abstractions;

public abstract class Message
{
    public DateTime Timestamp { get; protected set; } = DateTime.UtcNow;
}