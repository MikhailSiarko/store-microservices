using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Communication;
using Store.Infrastructure.Communication.Abstractions;
using Store.Services.User.Data;
using Store.Services.User.Domain;

namespace Store.Services.User.Application;

public static class IocExtensions
{
    public static IServiceCollection ConfigureApplication(this IServiceCollection services)
    {
        services.AddTransient<ICommunicationBus, CommunicationBus>();
        services.AddTransient<IUserService, UserService>();
        services.ConfigureData();
        services.ConfigureDomain();
        return services;
    }
}