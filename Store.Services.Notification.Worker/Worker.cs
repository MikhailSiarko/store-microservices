using Store.Infrastructure.Communication.Abstractions;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Worker;

public class Worker(ILogger<Worker> logger, ICommunicationBus bus) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await bus.SubscribeAsync(stoppingToken);
        while (!stoppingToken.IsCancellationRequested)
        {
            if (logger.IsEnabled(LogLevel.Information))
            {
                logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
            }

            await Task.Delay(1000, stoppingToken);
        }
    }
}