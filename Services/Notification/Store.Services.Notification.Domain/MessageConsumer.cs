using Store.Infrastructure.Communication.Abstractions;
using Store.Services.Shared.Messages.Notifications;
using Store.Services.Shared.Messages.User;

namespace Store.Services.Notification.Domain;

public sealed class MessageConsumer(
    ICommunicationBus bus,
    IReceiverInfoRepository receiverInfoRepository,
    INotificationRepository notificationRepository)
    : IMessageConsumer<UserCreated>, IMessageConsumer<NotificationCreated>
{
    public async Task HandleAsync(UserCreated message, CancellationToken token = default)
    {
        _ = await receiverInfoRepository.AddAsync(new ReceiverInfo
        {
            Email = message.Email,
            UserId = message.Id,
        }, token);

        var notification = new Notification
        {
            Email = message.Email,
            UserId = message.Id,
            Title = "Welcome to the Store",
            Body = "Thank you for joining us!"
        };

        notification = await notificationRepository.AddAsync(notification, token);
        if (notification is null)
            return;

        await bus.PublishAsync(new NotificationCreated { NotificationId = notification.Id }, token);
    }

    public async Task HandleAsync(NotificationCreated message, CancellationToken token = default)
    {
        var notification = await notificationRepository.GetAsync(message.NotificationId, token);
        if (notification is null || notification.IsSent)
        {
            return;
        }

        // sending...
        await Task.Delay(2000, token);

        notification.SentAt = DateTime.UtcNow;
        await notificationRepository.MarkAsSent(notification, token);
    }
}