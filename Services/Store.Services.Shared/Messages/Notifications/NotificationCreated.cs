using Store.Infrastructure.Communication.Abstractions;

namespace Store.Services.Shared.Messages.Notifications;

public class NotificationCreated : Message
{
    public Guid NotificationId { get; set; }
}