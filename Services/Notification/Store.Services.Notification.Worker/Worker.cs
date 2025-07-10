using Store.Infrastructure.Communication.Abstractions;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Worker;

public class Worker(
    [FromKeyedServices("UserEvents")] IBus userEventBus,
    [FromKeyedServices("NotificationEvents")] IBus notificationEventBus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.WhenAll(userEventBus.SubscribeAsync<MessageHandler>(stoppingToken),
            notificationEventBus.SubscribeAsync<MessageHandler>(stoppingToken));
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}