using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Communication.DependencyInjection;
using Store.Infrastructure.Communication.Implementations;
using Store.Services.User.Domain.Service;

namespace Store.Services.User.Domain;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddDomain(this IServiceCollection services, CommunicationOptions[] options)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddInfrastructure(options);
        return services;
    }
}