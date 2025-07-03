using Microsoft.Extensions.DependencyInjection;

namespace Store.Infrastructure.Communication.Abstractions;

public static class IocExtensions
{
    public static IServiceCollection ConfigureCommunication<T>(this IServiceCollection services)
    {
        services.AddMediatR(x => x.RegisterServicesFromAssembly(typeof(T).Assembly));
        return services;
    }
}