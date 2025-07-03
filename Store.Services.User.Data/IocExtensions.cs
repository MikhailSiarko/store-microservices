using Microsoft.Extensions.DependencyInjection;
using Store.Services.User.Domain;

namespace Store.Services.User.Data;

public static class IocExtensions
{
    public static IServiceCollection ConfigureData(this IServiceCollection services)
    {
        services.AddDbContext<UserDbContext>();
        services.AddTransient<IUserRepository, UserRepository>();
        return services;
    }
}