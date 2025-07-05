using Microsoft.Extensions.DependencyInjection;
using Store.Services.User.Domain;
using Store.Services.User.Domain.Storage;

namespace Store.Services.User.Data;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddTransient<IUserRepository, UserRepository>();
        return services;
    }
}