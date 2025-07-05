using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Store.Services.Notification.Domain;

namespace Store.Services.Notification.Data;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<NotificationDbContext>(x => x.UseSqlServer(configuration.GetConnectionString("NotificationDb")));
        services.AddTransient<INotificationRepository, NotificationRepository>();
        services.AddTransient<IReceiverInfoRepository, ReceiverInfoRepository>();
        return services;
    }
}