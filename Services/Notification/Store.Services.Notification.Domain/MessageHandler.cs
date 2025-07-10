using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Communication.Abstractions;
using Store.Services.Shared.Messages.Notifications;
using Store.Services.Shared.Messages.User;

namespace Store.Services.Notification.Domain;

public sealed class MessageHandler(
    [FromKeyedServices("NotificationSender")] IBus notificationSenderBus,
    IReceiverInfoRepository receiverInfoRepository,
    INotificationRepository notificationRepository)
    : IMessageHandler<UserCreated>, IMessageHandler<NotificationSent>
{
    public async Task HandleAsync(UserCreated message, CancellationToken token = default)
    {
        _ = await receiverInfoRepository.AddAsync(new ReceiverInfo
        {
            Id = Guid.CreateVersion7(),
            Email = message.Email,
            UserId = message.Id,
        }, token);

        var notification = new Notification
        {
            Id = Guid.CreateVersion7(),
            Email = message.Email,
            UserId = message.Id,
            Title = "Welcome to the Store",
            Body = "Thank you for joining us!"
        };

        notification = await notificationRepository.AddAsync(notification, token);
        if (notification is null)
            return;

        await notificationSenderBus.PublishAsync(new NotificationCreated { Id = notification.Id }, token);
    }

    public async Task HandleAsync(NotificationSent message, CancellationToken token = default)
    {
        var notification = await notificationRepository.GetAsync(message.Id, token);
        if (notification is null || notification.IsSent)
        {
            return;
        }

        notification.SentAt = message.SentAt;
        await notificationRepository.MarkAsSent(notification, token);
    }
}