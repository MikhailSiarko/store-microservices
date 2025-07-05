using Store.Infrastructure.Communication.Abstractions;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Worker;

public class Worker(ICommunicationBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await bus.SubscribeAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}