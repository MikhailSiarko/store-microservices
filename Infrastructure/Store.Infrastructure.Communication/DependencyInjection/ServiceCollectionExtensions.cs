using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Store.Infrastructure.Communication.Abstractions;
using Store.Infrastructure.Communication.Implementations;

namespace Store.Infrastructure.Communication.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, CommunicationOptions[] options)
    {
        foreach (var option in options)
        {
            services.AddKeyedSingleton<IBus, Bus>(option.TopicName, (sp, _) =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var environment = sp.GetRequiredService<IHostEnvironment>();
                var serviceScopeFactory = sp.GetRequiredService<IServiceScopeFactory>();
                return new Bus(option, configuration, environment, serviceScopeFactory);
            });
        }
        return services;
    }
}