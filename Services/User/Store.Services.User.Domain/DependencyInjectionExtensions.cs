using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.DependencyInjection;
using Store.Services.User.Domain.Service;

namespace Store.Services.User.Domain;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddInfrastructure();
        return services;
    }
}