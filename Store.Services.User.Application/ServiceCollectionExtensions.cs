using Microsoft.Extensions.DependencyInjection;
using Store.Services.User.Data;
using Store.Services.User.Domain;

namespace Store.Services.User.Application;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddData();
        services.AddDomain();
        return services;
    }
}