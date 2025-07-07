using Microsoft.Extensions.DependencyInjection;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Data;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddTransient<INotificationRepository, NotificationRepository>();
        services.AddTransient<IReceiverInfoRepository, ReceiverInfoRepository>();
        return services;
    }
}