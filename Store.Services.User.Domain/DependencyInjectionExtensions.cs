using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.DependencyInjection;

namespace Store.Services.User.Domain;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddInfrastructure();
        return services;
    }
}