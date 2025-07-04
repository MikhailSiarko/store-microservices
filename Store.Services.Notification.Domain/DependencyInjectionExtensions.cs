using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Communication.Abstractions;
using Store.Infrastructure.DependencyInjection;
using Store.Services.Shared.Messages.User;

namespace Store.Services.Notification.Domain;

public static class DependencyInjectionExtensions
{
    public static Task SubscribeAsync(this ICommunicationBus bus, CancellationToken token = default)
    {
        return bus.SubscribeAsync<UserCreated, IMessageConsumer<UserCreated>>(token);
    }

    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddTransient<IMessageConsumer<UserCreated>, UserMessageConsumer>();
        services.AddInfrastructure();
        return services;
    }
}