using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Communication.Abstractions;
using Store.Infrastructure.Communication.DependencyInjection;
using Store.Infrastructure.Communication.Implementations;
using Store.Services.Shared.Messages.Notifications;
using Store.Services.Shared.Messages.User;

namespace Store.Services.Notification.Domain;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services, CommunicationOptions[] options)
    {
        services.AddTransient<IMessageHandler<UserCreated>, MessageHandler>();
        services.AddTransient<IMessageHandler<NotificationSent>, MessageHandler>();
        services.AddInfrastructure(options);
        return services;
    }
}