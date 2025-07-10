using Store.Infrastructure.Communication.Abstractions;

namespace Store.Services.Shared.Messages.Notifications;

public class NotificationSent : Message
{
    public Guid Id { get; set; }
    public DateTime SentAt { get; set; }
}