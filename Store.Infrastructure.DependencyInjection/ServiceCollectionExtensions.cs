using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Communication;
using Store.Infrastructure.Communication.Abstractions;

namespace Store.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services,
        params Action<ICommunicationBus>[] subscriptions)
    {
        services.AddSingleton<ICommunicationBus, CommunicationBus>(sp =>
        {
            var scopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
            var configuration = sp.GetRequiredService<IConfiguration>();
            var bus = new CommunicationBus(configuration, scopeFactory);
            foreach (var subscription in subscriptions)
            {
                subscription(bus);
            }

            return bus;
        });
        return services;
    }
    
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICommunicationBus, CommunicationBus>();
        return services;
    }
}