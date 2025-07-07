using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Communication;
using Store.Infrastructure.Communication.Abstractions;

namespace Store.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddSingleton<ICommunicationBus, CommunicationBus>();
        return services;
    }
}