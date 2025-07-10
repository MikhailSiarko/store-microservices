namespace Store.Infrastructure.Communication.Implementations;

// ReSharper disable once ClassNeverInstantiated.Global
public class CommunicationOptions
{
    public string TopicName { get; set; } = null!;
    public string[]? Subscriptions { get; set; } = [];
}