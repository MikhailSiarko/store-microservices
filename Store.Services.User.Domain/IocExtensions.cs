using Microsoft.Extensions.DependencyInjection;
using Store.Infrastructure.Communication.Abstractions;
using Store.Services.User.Domain.Commands;

namespace Store.Services.User.Domain;

public static class IocExtensions
{
    public static IServiceCollection ConfigureDomain(this IServiceCollection services)
    {
        services.ConfigureCommunication<User>();
        services.AddTransient<ICommandHandler<CreateUserCommand>, UserCommandHandler>();
        return services;
    }
}