using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Communication.Abstractions;
using Store.Infrastructure.DependencyInjection;
using Store.Services.Shared.Messages.Notifications;
using Store.Services.Shared.Messages.User;

namespace Store.Services.Notification.Domain;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddTransient<IMessageConsumer<UserCreated>, MessageConsumer>();
        services.AddTransient<IMessageConsumer<NotificationCreated>, MessageConsumer>();
        services.AddInfrastructure();
        return services;
    }
}