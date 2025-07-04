using Microsoft.Extensions.DependencyInjection;
using Store.Services.User.Domain;

namespace Store.Services.User.Data;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddData(this IServiceCollection services)
    {
        services.AddDbContext<UserDbContext>();
        services.AddTransient<IUserRepository, UserRepository>();
        return services;
    }
}