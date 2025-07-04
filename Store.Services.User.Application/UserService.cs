using Store.Infrastructure.Communication.Abstractions;
using Store.Services.Shared.Messages.User;
using Store.Services.User.Application.Models;
using Store.Services.User.Domain;

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

    public async Task CreateUserAsync(CreateUserModel model, CancellationToken token = default)
    {
        var user = new Domain.User
        {
            Email = model.Email,
            FirstName = model.FirstName,
            LastName = model.LastName
        };

        if (!UserValidator.IsCreateCommandValid(user))
        {
            return;
        }

        user = await repository.AddAsync(user, token);
        if (user is null)
            return;

        await bus.PublishAsync(new UserCreated
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt
        }, token);
    }
}