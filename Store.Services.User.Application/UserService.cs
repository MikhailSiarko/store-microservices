using Store.Infrastructure.Communication.Abstractions;
using Store.Services.User.Application.Models;
using Store.Services.User.Domain;
using Store.Services.User.Domain.Commands;

namespace Store.Services.User.Application;

public sealed class UserService(ICommunicationBus bus, IUserRepository repository) : IUserService
{
    public async Task<UserModel[]> GetUsersAsync(CancellationToken token = default)
    {
        var users = await repository.GetAllAsync(token);
        return users.Select(x => new UserModel
        {
            Id = x.Id,
            Email = x.Email,
            FirstName = x.FirstName,
            LastName = x.LastName,
            CreatedAt = x.CreatedAt
        }).ToArray();
    }

    public Task CreateUserAsync(CreateUserModel model, CancellationToken token = default)
    {
        return bus.SendAsync(new CreateUserCommand
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName,
        }, token);
    }
}